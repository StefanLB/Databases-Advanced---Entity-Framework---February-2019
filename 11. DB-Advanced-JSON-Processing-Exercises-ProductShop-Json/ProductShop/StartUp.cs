using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        private const string path = @"C:\Users\StefanLB\source\repos\ProductShop\ProductShop\Datasets";

        public static void Main(string[] args)
        {

            using (var context = new ProductShopContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //string result = string.Empty;

                //result = ImportUsers(context, File.ReadAllText(path + @"\users.json"));
                //result = ImportProducts(context, File.ReadAllText(path + @"\products.json"));
                //result = ImportCategories(context, File.ReadAllText(path + @"\categories.json"));
                //result = ImportCategoryProducts(context, File.ReadAllText(path + @"\categories-products.json"));

                //Console.WriteLine(result);

                //Console.WriteLine(GetProductsInRange(context));
                //Console.WriteLine(GetSoldProducts(context));
                //Console.WriteLine(GetCategoriesByProductsCount(context));
                Console.WriteLine(GetUsersWithProducts(context));

            }

        }

        //Query 1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.Users.AddRange(users);

            var seededUsers = context.SaveChanges();

            return $"Successfully imported {seededUsers}";
        }

        //Query 2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.Products.AddRange(products);

            var seededProducts = context.SaveChanges();

            return $"Successfully imported {seededProducts}";
        }

        //Query 3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                .Where(x => x.Name != null)
                .ToArray();

            context.Categories.AddRange(categories);

            var seededCategories = context.SaveChanges();

            return $"Successfully imported {seededCategories}";
        }

        //Query 4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.CategoryProducts.AddRange(categoriesProducts);

            var seededCategoriesProducts = context.SaveChanges();

            return $"Successfully imported {seededCategoriesProducts}";
        }

        //Query 5. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var values = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = $"{x.Seller.FirstName} {x.Seller.LastName}"
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(values, Formatting.Indented);

            return json;
        }

        //Query 6. Export Successfully Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var values = context.Users
                .Where(x => x.ProductsSold.Any(sp => sp.Buyer != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new
                        {
                            name = p.Name,
                            price = p.Price,
                            buyerFirstName = p.Buyer.FirstName,
                            buyerLastName = p.Buyer.LastName
                        })
                        .ToList()
                })
                .OrderBy(x => x.lastName)
                .ThenBy(x => x.firstName)
                .ToList();

            return JsonConvert.SerializeObject(values, Formatting.Indented);
        }

        //Query 7. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var values = context.Categories
                .Select(x => new
                {
                    x.Name,
                    Prices = x.CategoryProducts.Select(cp => cp.Product.Price).ToList()
                })
                .ToList()
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.Prices.Count,
                    averagePrice = (x.Prices.Sum() / x.Prices.Count).ToString("F2"),
                    totalRevenue = (x.Prices.Sum()).ToString("F2")
                })
                .OrderByDescending(x => x.productsCount)
                .ToList();

            return JsonConvert.SerializeObject(values, Formatting.Indented);
        }

        //Query 8. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var values = context
                .Users
                .Where(u => u.ProductsSold.Any(ps=>ps.Buyer!=null))
                .OrderByDescending(u=> u.ProductsSold.Count(ps=>ps.Buyer !=null))
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new
                    {
                        Count = x.ProductsSold
                            .Count(ps => ps.Buyer != null),
                        Products = x.ProductsSold
                            .Where(ps => ps.Buyer != null)
                            .Select(ps => new
                            {
                                Name = ps.Name,
                                Price = ps.Price
                            })
                        .ToArray()
                    }
                })
                .ToArray();

            var result = new
            {
                UsersCount = values.Length,
                Users = values
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            return JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = contractResolver
            });
        }
    }
}