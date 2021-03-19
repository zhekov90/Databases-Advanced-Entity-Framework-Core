using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using AutoMapper;
using System.Xml.Linq;
using System.Xml;
using CarDealer.XmlHelper;

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

            //01.Import Users
            var result = ImportUsers(context, usersXml);

            Console.WriteLine(result);
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