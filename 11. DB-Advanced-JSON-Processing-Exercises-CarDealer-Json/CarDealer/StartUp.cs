using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        private const string path = @"C:\Users\StefanLB\source\repos\CarDealer\CarDealer\Datasets";

        public static void Main(string[] args)
        {
            using (var context = new CarDealerContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                string result = string.Empty;

                //result = ImportSuppliers(context, File.ReadAllText(path + @"\suppliers.json"));
                //result = ImportParts(context, File.ReadAllText(path + @"\parts.json"));
                //result = ImportCars(context, File.ReadAllText(path + @"\cars.json"));
                //result = ImportCustomers(context, File.ReadAllText(path + @"\customers.json"));
                //result = ImportSales(context, File.ReadAllText(path + @"\sales.json"));

                //result = GetOrderedCustomers(context);
                //result = GetCarsFromMakeToyota(context);
                //result = GetLocalSuppliers(context);
                //result = GetCarsWithTheirListOfParts(context);
                //result = GetTotalSalesByCustomer(context);
                result = GetSalesWithAppliedDiscount(context);

                Console.WriteLine(result);
            }
        }


        //Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);

            var seededSuppliers = context.SaveChanges();

            return $"Successfully imported {seededSuppliers}.";
        }

        //Query 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var suppliersCount = context.Suppliers.Count();

            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                .Where(s => s.SupplierId < suppliersCount)
                .ToArray();

            context.Parts.AddRange(parts);

            var seededParts = context.SaveChanges();

            return $"Successfully imported {seededParts}.";
        }

        //Query 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var values = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            var cars = new List<Car>();
            var partsCar = new List<PartCar>();

            int partsCount = context.Parts.Count();

            foreach (var value in values)
            {
                var car = new Car()
                {
                    Make = value.Make,
                    Model = value.Model,
                    TravelledDistance = value.TravelledDistance
                };

                foreach (var part in value.PartsId.Distinct())
                {
                    if (part<=partsCount)
                    {
                        partsCar.Add(new PartCar()
                        {
                            Car = car,
                            PartId = part
                        });
                    }
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);

            var seededCars = context.SaveChanges();

            context.PartCars.AddRange(partsCar);

            context.SaveChanges();

            return $"Successfully imported {seededCars}.";
        }

        //Query 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var values = JsonConvert.DeserializeObject<CustomerDto[]>(inputJson);
            var customers = new List<Customer>();

            foreach (var value in values)
            {
                customers.Add(new Customer()
                {
                    Name = value.Name,
                    BirthDate = value.BirthDate,
                    IsYoungDriver = value.IsYoungDriver
                });
            }

            context.Customers.AddRange(customers);

            var seededCustomers = context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        //Query 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var values = JsonConvert.DeserializeObject<SalesDto[]>(inputJson);

            var sales = new List<Sale>();

            foreach (var value in values)
            {
                sales.Add(new Sale()
                {
                    CarId = value.CarId,
                    CustomerId = value.CustomerId,
                    Discount = value.Discount
                });
            }

            context.Sales.AddRange(sales);

            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        //Query 14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var values = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c=> new CustomerExportDto()
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            //DefaultContractResolver contractResolver = new DefaultContractResolver()
            //{
            //    NamingStrategy = new CamelCaseNamingStrategy()
            //};

            var json = JsonConvert.SerializeObject(values, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                //ContractResolver = contractResolver
            });

            return json;
        }

        //Query 15. Export Cars from make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var values = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new CarExportDto()
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(values, Formatting.Indented);

            return json;
        }

        //Query 16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var values = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new SupplierExportDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            var json = JsonConvert.SerializeObject(values, Formatting.Indented);

            return json;
        }

        //Query 17. Export Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x => new
                {
                    car = new
                    {
                        x.Make,
                        x.Model,
                        x.TravelledDistance
                    },
                    parts = x.PartCars.Select(p => new
                    {
                        p.Part.Name,
                        Price = p.Part.Price.ToString("F2")
                    })
                    .ToList()
                })
                .ToList();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        //Query 18. Export Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Any())
                .Select(x => new
                {
                    fullName = x.Name,
                    boughtCars = x.Sales.Count,
                    sales = x.Sales.Select(s => new
                    {
                        Price = s.Car.PartCars.Select(pc => pc.Part.Price).Sum(),
                        s.Discount
                    })
                    .ToList()
                })
                .ToList()
                .Select(x => new
                {
                    x.fullName,
                    x.boughtCars,
                    spentMoney = x.sales.Sum(s => s.Price)
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        //Query 19. Export Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(x => new
                {
                    car = x.Car,
                    customerName = x.Customer.Name,
                    x.Discount,
                    price = x.Car.PartCars.Select(pc => pc.Part.Price).Sum()
                })
                .Take(10)
                .ToArray()
                .Select(x => new
                {
                    car = new
                    {
                        x.car.Make,
                        x.car.Model,
                        x.car.TravelledDistance
                    },
                    x.customerName,
                    Discount = x.Discount.ToString("F2"),
                    price = x.price.ToString("F2"),
                    priceWithDiscount = (x.price - ((x.Discount / 100.0m) * x.price)).ToString("F2")
                })
                .ToArray();


            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }

    }
}