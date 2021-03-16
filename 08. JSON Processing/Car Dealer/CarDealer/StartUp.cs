using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //09. Import Suppliers
            //var json = File.ReadAllText("../../../Datasets/suppliers.json");
            //ImportSuppliers(context, json);

            //10. Import Parts
            var json = File.ReadAllText("../../../Datasets/parts.json");
            ImportParts(context, json);



        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var suppliedIds = context.Suppliers.Select(x => x.Id).ToArray();

            var parts = JsonConvert.DeserializeObject<IEnumerable<Part>>(inputJson)
                .Where(s=>suppliedIds.Contains(s.SupplierId))
                .ToList();

            context.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliersDtos = JsonConvert.DeserializeObject<IEnumerable<ImportSupplierInputModel>>(inputJson);

            var suppliers = suppliersDtos.Select(x => new Supplier
            {
                Name = x.Name,
                IsImporter = x.IsImporter
            })
                .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }
    }
}