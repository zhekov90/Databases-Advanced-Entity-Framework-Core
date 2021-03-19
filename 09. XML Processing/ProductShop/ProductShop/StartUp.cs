using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.IO;
using System.Linq;
using CarDealer.XmlHelper;
using System.Xml.Serialization;
using System.Collections.Generic;
using AutoMapper;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var usersXml = File.ReadAllText("../../../Datasets/users.xml");
            var productsXml = File.ReadAllText("../../../Datasets/products.xml");

            //01.Import Users
            ImportUsers(context, usersXml);

            //02. Import Products
            var result = ImportProducts(context, productsXml);

            Console.WriteLine(result);
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            const string root = "Products";

            var productDtos = XmlConverter.Deserializer<ProductImportModel>(inputXml, root);

            var products = productDtos
                .Select(x => new Product
                {
                    Name = x.Name,
                    Price = x.Price,
                    BuyerId = x.BuyerId,
                    SellerId = x.SellerId
                })
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {

            const string root = "Users";

            var userDtos = XmlConverter.Deserializer<UserImportModel>(inputXml, root);

            var users = userDtos
                .Select(x => new User
                {
                    Age = x.Age,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                })
                .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
    }
}