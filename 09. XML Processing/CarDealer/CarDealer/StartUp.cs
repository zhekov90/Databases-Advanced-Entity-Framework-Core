using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Input;
using CarDealer.DTOs.Output;
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

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var supplierXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //var partsXml = File.ReadAllText("../../../Datasets/parts.xml");
            //var carsXml = File.ReadAllText("../../../Datasets/cars.xml");
            //var customersXml = File.ReadAllText("../../../Datasets/customers.xml");
            //var salesXml = File.ReadAllText("../../../Datasets/sales.xml");

            //09. Import Suppliers
            //ImportSuppliers(context, supplierXml);
            //var result = ImportSuppliers(context, supplierXml);

            //10. Import Parts
            //ImportParts(context, partsXml);
            //var result = ImportParts(context, partsXml);

            //11. Import Cars
            //ImportCars(context, carsXml);
            //var result = ImportCars(context, carsXml);

            //12. Import Customers
            //ImportCustomers(context, customersXml);
            //var result = ImportCustomers(context, customersXml);

            //13. Import Sales
            //ImportSales(context, salesXml);
            //var result = ImportCustomers(context, salesXml);

            //14. Export Cars With Distance
            //var result = GetCarsWithDistance(context);

            //15. Export Cars From Make BMW
            //var result = GetCarsFromMakeBmw(context);

            //16. Export Local Suppliers
            var result = GetLocalSuppliers(context);
            Console.WriteLine(result);

        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            const string root = "suppliers";

            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new LocalSupplierOutputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count()
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LocalSupplierOutputModel[]), new XmlRootAttribute(root));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, suppliers, ns);

            var result = textWriter.ToString();
            return result;
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            const string root = "cars";

            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .Select(x => new BmwOutputModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(BmwOutputModel[]), new XmlRootAttribute(root));

            var textWriter = new StringWriter();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(textWriter, cars, namespaces);

            var result = textWriter.ToString();
            return result;
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            const string root = "cars";

            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2000000)
                .Select(x=>new CarOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarOutputModel[]), new XmlRootAttribute(root));

            var textWriter = new StringWriter();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            xmlSerializer.Serialize(textWriter, cars, namespaces);

            var result = textWriter.ToString();

            return result;
            
        }
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            const string root = "Sales";
            var validCars = context.Cars.Select(x=>x.Id).ToList();

            var salesDto = XmlConverter.Deserializer<SaleInputModel>(inputXml, root);

            var sales = salesDto
                .Where(x=>validCars.Contains(x.CarId))
                .Select(x => new Sale
                {
                    CarId = x.CarId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                })
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
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
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}