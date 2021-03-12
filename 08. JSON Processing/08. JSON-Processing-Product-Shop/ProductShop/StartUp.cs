using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext db = new ProductShopContext();

            //ResetDatabase(db);

            //01. Import Users
            //string inputJson = File.ReadAllText("../../../Datasets/users.json");
            //var result = ImportUsers(db, inputJson);
            //Console.WriteLine(result);

            //02. Import Products
            //string inputJson = File.ReadAllText("../../../Datasets/products.json");
            //var result = ImportProducts(db, inputJson);
            //Console.WriteLine(result);

            //03. Import Categories
            //string inputJson = File.ReadAllText("../../../Datasets/categories.json");
            //var result = ImportCategories(db, inputJson);
            //Console.WriteLine(result);

            //04. Import Categories and Products
            string inputJson = File.ReadAllText("../../../Datasets/categories-products.json");
            var result = ImportCategoryProducts(db, inputJson);
            Console.WriteLine(result);


        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                .Where(c=>c.Name != null)
                .ToArray();

            context.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson);

            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            User[] users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        private static void ResetDatabase(ProductShopContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("Database was successfully deleted!");
            db.Database.EnsureCreated();
            Console.WriteLine("Database was successfully created!");
        }

    }
}