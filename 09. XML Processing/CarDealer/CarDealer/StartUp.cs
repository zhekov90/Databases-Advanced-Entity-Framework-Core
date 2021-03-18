using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Input;
using CarDealer.Models;
using CarDealer.XmlHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var supplierXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            var partsXml = File.ReadAllText("../../../Datasets/parts.xml");
            var carsXml = File.ReadAllText("../../../Datasets/cars.xml");
            var customersXml = File.ReadAllText("../../../Datasets/customers.xml");

            //09. Import Suppliers
            ImportSuppliers(context, supplierXml);
            //var result = ImportSuppliers(context, supplierXml);

            //10. Import Parts
            ImportParts(context, partsXml);
            //var result = ImportParts(context, partsXml);

            //11. Import Cars
            ImportCars(context, carsXml);
            //var result = ImportCars(context, carsXml);

            //12. Import Customers
            ImportCustomers(context, customersXml);
            var result = ImportCustomers(context, customersXml);


            Console.WriteLine(result);

        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            const string root = "Customers";
            InitializeAutomapper();

            var customersDto = XmlConverter.Deserializer<CustomerInputModel>(inputXml, root);

            var customers = customersDto
                .Select(x => new Customer
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            const string root = "Cars";

            var carsDto = XmlConverter.Deserializer<CarInputModel>(inputXml, root);

            var cars = new List<Car>();

            var allParts = context.Parts.Select(x => x.Id).ToList();

            foreach (var currentCar in carsDto)
            {
                var distinctedParts = currentCar.Parts.Select(x => x.Id).Distinct();
                var parts = distinctedParts.Intersect(allParts);

                var car = new Car
                {
                    Make = currentCar.Make,
                    Model = currentCar.Model,
                    TravelledDistance = currentCar.TraveledDistance
                };

                foreach (var part in parts)
                {
                    var partCar = new PartCar
                    {
                        PartId = part
                    };

                    car.PartCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            const string root = "Parts";

            var partsDto = XmlConverter.Deserializer<PartInputModel>(inputXml, root);

            var supplierIds = context.Suppliers
                .Select(x => x.Id)
                .ToList();

            var parts = partsDto
                .Where(s => supplierIds.Contains(s.SupplierId))
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId
                })
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            const string root = "Suppliers";

            var suppliersDto = XmlConverter.Deserializer<SupplierInputModel>(inputXml, root);

            var suppliers = suppliersDto
                .Select(x => new Supplier
                {
                    Name = x.Name,
                    IsImporter = x.IsImporter
                })
                .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        private static void InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<CarDealerProfile>();
        });

         mapper = config.CreateMapper();
        }
    }
}