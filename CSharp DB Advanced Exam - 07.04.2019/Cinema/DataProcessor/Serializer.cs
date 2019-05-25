namespace Cinema.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .Where(m => m.Rating >= rating)
                .Where(m => m.Projections.Any(p => p.Tickets.Count > 0))
                .OrderByDescending(m => m.Rating)
                .ThenByDescending(x => x.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Take(10)
                .Select(x => new ExportMoviesDto
                {
                    MovieName = x.Title,
                    Rating = $"{x.Rating:F2}",
                    TotalIncomes = $"{x.Projections.Sum(p => p.Tickets.Sum(t => t.Price)):F2}",
                    Customers = x.Projections.SelectMany(c => c.Tickets)
                    .Select(t => new ExportCustomerDto
                    {
                        FirstName = t.Customer.FirstName,
                        LastName = t.Customer.LastName,
                        Balance = $"{t.Customer.Balance:F2}"
                    })
                    .OrderByDescending(b => b.Balance)
                    .ThenBy(f => f.FirstName)
                    .ThenBy(l => l.LastName)
                    .ToArray()
                });


            var jsonResult = JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);

            return jsonResult;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context.Customers
                .Where(c => c.Age >= age)
                .OrderByDescending(c => c.Tickets.Sum(p => p.Price))
                .Take(10)
                .Select(c => new ExportCustomersDto
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = $"{c.Tickets.Sum(p => p.Price):F2}",
                    SpentTime = TimeSpan.FromTicks(c.Tickets.Sum(t => t.Projection.Movie.Duration.Ticks)).ToString(@"hh\:mm\:ss")
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCustomersDto[]), new XmlRootAttribute("Customers"));
            var namespaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });

            var sb = new StringBuilder();
            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            var result = sb.ToString().TrimEnd();

            return result;

            /*
             <Customers>
              <Customer FirstName="Marjy" LastName="Starbeck">
                <SpentMoney>82.65</SpentMoney>
                <SpentTime>17:04:00</SpentTime>
              </Customer>
             */

            //For each customer, export their first name, last name, spent money for tickets (formatted to the second digit) and spent time (in format: "hh\:mm\:ss"). Take first 10 records and order the result by spent money in descending order.
        }
    }
}