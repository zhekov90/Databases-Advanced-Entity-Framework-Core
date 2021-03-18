using CarDealer.Data;
using CarDealer.DTOs.Input;
using CarDealer.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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
            var supplierXml = File.ReadAllText("../../../Datasets/suppliers.xml");
           var result = ImportSuppliers(context, supplierXml);
           Console.WriteLine(result);

        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SupplierInputModel[]), new XmlRootAttribute("Suppliers"));

            var textRead = new StringReader(inputXml);

            var supplilersDto = xmlSerializer.Deserialize(textRead) as SupplierInputModel[];

            var suppliers = supplilersDto
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
    }
}