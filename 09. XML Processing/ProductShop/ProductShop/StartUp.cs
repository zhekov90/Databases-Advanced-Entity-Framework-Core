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
using ProductShop.Dtos.Export;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var usersXml = File.ReadAllText("../../../Datasets/users.xml");
            //var productsXml = File.ReadAllText("../../../Datasets/products.xml");
            //var categoriesXml = File.ReadAllText("../../../Datasets/categories.xml");
            //var categoriesProductsXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            //01.Import Users
            //ImportUsers(context, usersXml);

            //02. Import Products
            //ImportProducts(context, productsXml);

            //03. Import Categories
            //ImportCategories(context, categoriesXml);

            //04. Import Categories and Products
            //ImportCategoryProducts(context, categoriesProductsXml);

            //05. Export Products In Range
            //var result = GetProductsInRange(context);
            //File.WriteAllText("../../../results/products-in-range.xml", result);

            //06. Export Sold Products
            var result = GetSoldProducts(context);
            File.WriteAllText("../../../results/users-sold-products.xml", result);
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            const string root = "Users";

            var users = context.Users
                .Where(x => x.ProductsSold.Any())
                .Select(x => new UserSoldProductExport
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(ps => new SoldProductExport
                    {
                        Name = ps.Name,
                        Price = ps.Price
                    })
                    .ToArray()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToArray();

            var result = XmlConverter.Serialize(users, root);

            return result;
        }
        public static string GetProductsInRange(ProductShopContext context)
        {
            const string root = "Products";

            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new ProductExport
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToList();

            var result = XmlConverter.Serialize(products, root);

            return result;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            const string root = "CategoryProducts";

            var categoriesProductsDto = XmlConverter.Deserializer<CategoryProductImportModel>(inputXml, root);

            var validCategories = context.Categories
                .Select(x => x.Id);
            var validProducts = context.Products
                .Select(x => x.Id);

            var categoriesProducts = categoriesProductsDto
                .Where(x => validCategories.Contains(x.CategoryId) && validProducts.Contains(x.ProductId))
                .Select(x => new CategoryProduct
                {
                    CategoryId = x.CategoryId,
                    ProductId = x.ProductId
                })
                .ToArray();

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            const string root = "Categories";

            var categoriesDto = XmlConverter.Deserializer<CategoryImportModel>(inputXml, root);

            List<Category> categories = new List<Category>();

            foreach (var dto in categoriesDto)
            {
                if (dto == null)
                {
                    continue;
                }

                var category = new Category
                {
                    Name = dto.Name
                };

                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
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