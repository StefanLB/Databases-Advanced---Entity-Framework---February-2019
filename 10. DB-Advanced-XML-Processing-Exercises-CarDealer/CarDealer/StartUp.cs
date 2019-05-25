using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        private const string path = @"C:\Users\StefanLB\source\repos\CarDealer\CarDealer\DataSets";

        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<CarDealerProfile>();
            });

            using (var context = new CarDealerContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                string result = string.Empty;

                //result = ImportSuppliers(context, File.ReadAllText(path + @"\suppliers.xml"));
                //result = ImportParts(context, File.ReadAllText(path + @"\parts.xml"));
                //result = ImportCars(context, File.ReadAllText(path + @"\cars.xml"));
                //result = ImportCustomers(context, File.ReadAllText(path + @"\customers.xml"));
                //result = ImportSales(context, File.ReadAllText(path + @"\sales.xml"));

                //result = GetCarsWithDistance(context);
                //result = GetCarsFromMakeBmw(context);
                //result = GetLocalSuppliers(context);
                //result = GetCarsWithTheirListOfParts(context);
                //result = GetTotalSalesByCustomer(context);
                result = GetSalesWithAppliedDiscount(context);

                Console.WriteLine(result);
            }
        }


        //Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));

            var suppliersDto = (ImportSupplierDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var suppliers = new List<Supplier>();

            foreach (var supplierDto in suppliersDto)
            {
                var supplier = Mapper.Map<Supplier>(supplierDto);

                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        //Query 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));

            var partsDto = (ImportPartDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var parts = new List<Part>();
            var suppliersCount = context.Suppliers.Count();

            foreach (var partDto in partsDto)
            {
                if (partDto.SupplierId<= suppliersCount && partDto.SupplierId > 0)
                {
                    var part = Mapper.Map<Part>(partDto);

                    parts.Add(part);
                }
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        //Query 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]), new XmlRootAttribute("Cars"));

            var carsDtos = (ImportCarDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var cars = new List<Car>();
            var carParts = new List<PartCar>();

            foreach (var car in carsDtos)
            {
                var currentCar = Mapper.Map<Car>(car);

                foreach (var part in car.Parts)
                {
                    if (!currentCar.PartCars.Select(e => e.PartId).Contains(part.PartId)
                        && part.PartId <= context.Parts.Count())
                    {
                        currentCar.PartCars.Add(new PartCar()
                        {
                            PartId = part.PartId
                        });
                    }
                }
                cars.Add(currentCar);
            }

            context.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //Query 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));

            var customersDto = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = new List<Customer>();

            foreach (var customerDto in customersDto)
            {
                    var customer = Mapper.Map<Customer>(customerDto);

                    customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        //Query 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportSaleDto[]),
                new XmlRootAttribute("Sales"));

            var salesDeserialized = (ImportSaleDto[])serializer
                .Deserialize(new StringReader(inputXml));

            var sales = new List<Sale>();

            foreach (var sale in salesDeserialized.Where(e => context.Cars.Any(n => n.Id == e.CarId) && e.Discount <= 100))
            {
                var currentSale = Mapper.Map<Sale>(sale);
                sales.Add(currentSale);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        //Query 14. Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(e => e.TravelledDistance > 2000000)
                .OrderBy(e => e.Make)
                .ThenBy(e => e.Model)
                .Select(e => new ExportCarsWithDistanceDto
                {
                    Make = e.Make,
                    Model = e.Model,
                    TravelledDistance = e.TravelledDistance
                })
                .Take(10)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarsWithDistanceDto[]),
                new XmlRootAttribute("cars"));

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(new StringWriter(sb), cars, ns);

            return sb.ToString();
        }


        //Query 15. Cars from make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            //Get all cars from make BMW and order them by model alphabetically and by travelled distance descending.
            //Return the list of suppliers to XML in the format provided below.

            var cars = context.Cars
                .Where(e => e.Make == "BMW")
                .OrderBy(e => e.Model)
                .ThenByDescending(e => e.TravelledDistance)
                .Select(e => new ExportCarsFromMakeBMWDto
                {
                    Id = e.Id,
                    Model = e.Model,
                    TravelledDistance = e.TravelledDistance
                })
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarsFromMakeBMWDto[]),
                new XmlRootAttribute("cars"));

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(new StringWriter(sb), cars, ns);

            return sb.ToString();
        }


        //Query 16. Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            //Get all suppliers that do not import parts from abroad.Get their id, name and the number of parts they can offer to supply.
            //Return the list of suppliers to XML in the format provided below.

            var suppliers = context.Suppliers
               .Where(e => e.IsImporter == false)
               .Select(e => new ExportLocalSuppliersDto
               {
                   Id = e.Id,
                   Name = e.Name,
                   PartsCount = e.Parts.Count()
               })
               .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportLocalSuppliersDto[]),
                new XmlRootAttribute("suppliers"));

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(new StringWriter(sb), suppliers, ns);

            return sb.ToString();
        }


        //Query 17. Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {

            //Get all cars along with their list of parts.For the car get only make, model and 
            //travelled distance and for the parts get only name and price and sort all pars by 
            //price(descending).Sort all cars by travelled distance(descending) then by model(ascending).Select top 5 records.

            var cars = context.Cars
                 .OrderByDescending(e => e.TravelledDistance)
                 .ThenBy(e => e.Model)
                .Select(e => new ExportCarsWithListOfPartsDto
                {
                    Make = e.Make,
                    Model = e.Model,
                    TravelledDistance = e.TravelledDistance,

                    Parts =

                        e.PartCars.Select(p => new ExportCarsPartsListDto
                        {
                            Name = p.Part.Name,
                            Price = p.Part.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                })
                .Take(5)
                .ToArray();


            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarsWithListOfPartsDto[]),
                new XmlRootAttribute("cars"));

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(new StringWriter(sb), cars, ns);

            return sb.ToString();
        }


        //Query 18. Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            //Get all customers that have bought at least 1 car and get their names, bought cars count and 
            //total spent money on cars.Order the result list by total spent money descending.

            var customers = context.Customers
                 .Where(e => e.Sales.Any())
                  .OrderByDescending(e => e.Sales.Sum(s => s.Car.PartCars.Sum(n => n.Part.Price)))
                 .Select(e => new ExportTotalSalesByCustomerDto
                 {
                     Name = e.Name,
                     BoughtCars = e.Sales.Count(),
                    //SpentMoney = e.Sales.Select(s => s.Car.PartCars.Sum(n => n.Part.Price)),
                    SpentMoney = e.Sales.Sum(s => s.Car.PartCars.Sum(n => n.Part.Price)),

                 })
                 .ToArray();


            XmlSerializer serializer = new XmlSerializer(typeof(ExportTotalSalesByCustomerDto[]),
                new XmlRootAttribute("customers"));

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(new StringWriter(sb), customers, ns);

            return sb.ToString();
        }


        //Query 19. Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            //Get all sales with information about the car, customer and price of the sale with and without discount.

            var sales = context.Sales
                .Select(e => new ExportSalesWithAppliedDiscountDto
                {
                    Car = new ExportCarsAttributeDto
                    {
                        Make = e.Car.Make,
                        Model = e.Car.Model,
                        TravelledDistance = e.Car.TravelledDistance
                    },
                    Discount = e.Discount,
                    CustomerName = e.Customer.Name,
                    Price = e.Car.PartCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = ((e.Car.PartCars.Sum(p => p.Part.Price) * (1 - e.Discount / 100))).ToString().TrimEnd('0'),
                })
                .ToArray();

            //var sales2 = new TestDto
            //{
            //    sales = sales,
            //   ExportSaleDiscount = string.Empty
            //};

            XmlSerializer serializer = new XmlSerializer(typeof(ExportSalesWithAppliedDiscountDto[]),
                new XmlRootAttribute("sales"));

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(new StringWriter(sb), sales, ns);

            return sb.ToString();
        }
    }
}