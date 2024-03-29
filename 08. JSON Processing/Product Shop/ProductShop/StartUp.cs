﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs;
using ProductShop.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var db = new ProductShopContext();

            InitializeMapper();
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
            //string inputJson = File.ReadAllText("../../../Datasets/categories-products.json");
            //var result = ImportCategoryProducts(db, inputJson);
            //Console.WriteLine(result);

            //05. Export Products In Range
            //var result = GetProductsInRange(db);
            //File.WriteAllText("../../../Datasets/products-in-range.json", result);

            //06. Export Sold Products
            //var result = GetSoldProducts(db);
            //File.WriteAllText("../../../Datasets/users-sold-products.json", result);

            ////07. Export Categories By Products Count
            //var result = GetCategoriesByProductsCount(db);
            //File.WriteAllText("../../../Datasets/categories-by-products.json", result);


            //08. Export Users and Products
            var result = GetUsersWithProducts(db);
            Console.WriteLine(result);

        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Include(x=>x.ProductsSold)
                .ToList()
                .Where(x => x.ProductsSold.Any(b=>b.BuyerId !=null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Where(b => b.BuyerId != null).Count(),
                        products = x.ProductsSold.Where(b=>b.BuyerId != null).Select(p => new
                        {
                            name = p.Name,
                            price = p.Price
                        })
                        .ToList()
                    }
                })
                .ToList()
                .OrderByDescending(x => x.soldProducts.products.Count())
                .ToList();

            var resultObject = new
            {
                usersCount = users.Count(),
                users = users
            };

            var jsonSerializingOptions = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var resultJson = JsonConvert.SerializeObject(resultObject, Formatting.Indented, jsonSerializingOptions);

            return resultJson;
        }
    public static string GetCategoriesByProductsCount(ProductShopContext context)
    {
        var categoriesInfo = context.Categories
            .Select(x => new
            {
                category = x.Name,
                productsCount = x.CategoryProducts.Count(),
                averagePrice = x.CategoryProducts.Average(p => p.Product.Price).ToString("f2"),
                totalRevenue = x.CategoryProducts.Sum(p => p.Product.Price).ToString("f2")
            })
            .OrderByDescending(x => x.productsCount)
            .ToList();


        var result = JsonConvert.SerializeObject(categoriesInfo, Formatting.Indented);

        return result;
    }

    public static string GetSoldProducts(ProductShopContext context)
    {
        var users = context.Users
            .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
            .Select(x => new
            {
                firstName = x.FirstName,
                lastName = x.LastName,
                soldProducts = x.ProductsSold.Where(p => p.BuyerId != null).Select(b => new
                {
                    name = b.Name,
                    price = b.Price,
                    buyerFirstName = b.Buyer.FirstName,
                    buyerLastName = b.Buyer.LastName
                })
                .ToList()
            })
            .OrderBy(x => x.lastName)
            .ThenBy(x => x.firstName)
            .ToList();

        var result = JsonConvert.SerializeObject(users, Formatting.Indented);

        return result;
    }

    public static string GetProductsInRange(ProductShopContext context)
    {
        var products = context.Products
            .Where(p => p.Price >= 500 && p.Price <= 1000)
            .Select(x => new
            {
                name = x.Name,
                price = x.Price,
                seller = x.Seller.FirstName + " " + x.Seller.LastName
            })
            .OrderBy(x => x.price)
            .ToList();

        var result = JsonConvert.SerializeObject(products, Formatting.Indented);

        return result;
    }

    public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
    {
        var dtoCategoryProducts = JsonConvert.DeserializeObject<IEnumerable<CategoryProductInputModel>>(inputJson);

        var categoryProducts = Mapper.Map<IEnumerable<CategoryProduct>>(dtoCategoryProducts);

        context.CategoryProducts.AddRange(categoryProducts);
        context.SaveChanges();

        return $"Successfully imported {categoryProducts.Count()}";
    }

    public static string ImportCategories(ProductShopContext context, string inputJson)
    {
        var dtoCategories = JsonConvert.DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson)
            .Where(c => c.Name != null);

        var categories = Mapper.Map<IEnumerable<Category>>(dtoCategories);

        context.Categories.AddRange(categories);
        context.SaveChanges();

        return $"Successfully imported {categories.Count()}";
    }

    public static string ImportProducts(ProductShopContext context, string inputJson)
    {
        var products = JsonConvert.DeserializeObject<List<Product>>(inputJson);

        context.Products.AddRange(products);

        context.SaveChanges();

        return $"Successfully imported {products.Count}";
    }

    public static string ImportUsers(ProductShopContext context, string inputJson)
    {
        var serializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        var usersDTO = JsonConvert.DeserializeObject<List<UserInputModel>>(inputJson, serializerSettings);

        var users = usersDTO
            .Select(udto => Mapper.Map<User>(udto))
            .ToList();

        context.Users.AddRange(users);

        context.SaveChanges();

        return $"Successfully imported {users.Count}";
    }

    private static void InitializeMapper()
    {
        Mapper.Initialize(cfg =>
        {
            cfg.AddProfile<ProductShopProfile>();
        });
    }

    private static void ResetDatabase(ProductShopContext db)
    {
        db.Database.EnsureDeleted();
        Console.WriteLine("Db was successfully deleted!");

        db.Database.EnsureCreated();
        Console.WriteLine("Db was successfully created!");
    }
}
}