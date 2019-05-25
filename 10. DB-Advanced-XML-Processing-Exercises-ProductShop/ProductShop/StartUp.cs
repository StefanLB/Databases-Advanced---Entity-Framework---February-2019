using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DataAnotations = System.ComponentModel.DataAnnotations;

namespace ProductShop
{
    public class StartUp
    {
        private const string path = @"C:\Users\StefanLB\source\repos\ProductShop\ProductShop\DataSets";

        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<ProductShopProfile>();
            });


            using (var context = new ProductShopContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                string result = string.Empty;

                //result = ImportUsers(context, File.ReadAllText(path + @"\users.xml"));
                //result = ImportProducts(context, File.ReadAllText(path + @"\products.xml"));
                //result = ImportCategories(context, File.ReadAllText(path + @"\categories.xml"));
                //result = ImportCategoryProducts(context, File.ReadAllText(path+@"\categories-products.xml"));

                //result = GetProductsInRange(context);
                //result = GetSoldProducts(context);
                //result = GetCategoriesByProductsCount(context);
                result = GetUsersWithProducts(context);

                Console.WriteLine(result);
            }
        }


        //Query 1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]),new XmlRootAttribute("Users"));

            var usersDto = (ImportUserDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var users = new List<User>();

            foreach (var userDto in usersDto)
            {
                var user = Mapper.Map<User>(userDto);

                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        //Query 2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProductDto[]),new XmlRootAttribute("Products"));

            var productsDto = (ImportProductDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var products = new List<Product>();

            foreach (var productDto in productsDto)
            {
                var product = Mapper.Map<Product>(productDto);

                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //Query 3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoriesDto[]), new XmlRootAttribute("Categories"));

            var categoriesDto = (ImportCategoriesDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var categories = new List<Category>();

            foreach (var categoryDto in categoriesDto)
            {
                if (!IsValid(categoryDto))
                {
                    continue;
                }

                var category = Mapper.Map<Category>(categoryDto);

                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";

        }

        public static bool IsValid(object obj)
        {
            var validationContext = new DataAnotations.ValidationContext(obj);
            var validationsResult = new List<DataAnotations.ValidationResult>();

            var result = DataAnotations.Validator.TryValidateObject(obj, validationContext, validationsResult, true);

            return result;
        }

        //Query 4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductDto[]), new XmlRootAttribute("CategoryProducts"));

            var categoryProductsDto = (ImportCategoryProductDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var categoryProducts = new List<CategoryProduct>();

            var productsCount = context.Products.Count();
            var categoriesCount = context.Categories.Count();

            foreach (var categProd in categoryProductsDto)
            {
                if (categProd.CategoryId<=categoriesCount && categProd.CategoryId>0)
                {
                    if (categProd.ProductId<=productsCount && categProd.ProductId>0)
                    {
                        var categoryProduct = Mapper.Map<CategoryProduct>(categProd);

                        categoryProducts.Add(categoryProduct);
                    }
                }
            }

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        //Query 5. Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(x => new ExportProductsInRangeDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportProductsInRangeDto[]),new XmlRootAttribute("Products"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb),products,namespaces);

            return sb.ToString().TrimEnd();
        }

        //Query 6. Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any())
                .Select(x => new ExportSoldProductsDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ProductDto = x.ProductsSold.Select(p => new ProductDto
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToArray()
                })
                .OrderBy(l => l.LastName)
                .ThenBy(f => f.FirstName)
                .Take(5)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportSoldProductsDto[]), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });

            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }

        //Query 7. Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .Select(x => new ExportCategoriesByCountDto
                {
                    Name = x.Name,
                    ProductsCount = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Select(a => a.Product.Price).Average(),
                    TotalRevenue = x.CategoryProducts.Select(а => а.Product.Price).Sum()
                })
                .OrderByDescending(x => x.ProductsCount)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();

            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ExportCategoriesByCountDto[]), new XmlRootAttribute("Categories"));
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            serializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }

        //Query 8. Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(x => x.ProductsSold.Any())
                .Select(x => new ExportUserAndProductDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProductDto = new SoldProductDto
                    {
                        Count = x.ProductsSold.Count,
                        ProductDtos = x.ProductsSold.Select(p => new ProductDto()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(x => x.SoldProductDto.Count)
                .Take(10)
                .ToArray();

            var customExport = new ExportCustomUserProductDto
            {
                Count = context.Users.Count(x => x.ProductsSold.Any()),
                ExportUserAndProductDto = users
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCustomUserProductDto), new XmlRootAttribute("Users"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var sb = new StringBuilder();

            xmlSerializer.Serialize(new StringWriter(sb), customExport, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}