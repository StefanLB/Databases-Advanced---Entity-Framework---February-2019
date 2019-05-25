namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var moviesDto = JsonConvert.DeserializeObject<ImportMovieDto[]>(jsonString);

            var sb = new StringBuilder();
            var movies = new List<Movie>();

            foreach (var movieDto in moviesDto)
            {
                bool isValidEnum = Enum.TryParse<Genre>(movieDto.Genre, out Genre test);

                if (!IsValid(movieDto) || !isValidEnum)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var movie = new Movie
                {
                    Title = movieDto.Title,
                    Genre = Enum.Parse<Genre>(movieDto.Genre),
                    Duration = TimeSpan.ParseExact(movieDto.Duration, @"hh\:mm\:ss", CultureInfo.InvariantCulture),
                    Rating = movieDto.Rating,
                    Director = movieDto.Director
                };

                movies.Add(movie);
                sb.AppendLine(String.Format(SuccessfulImportMovie, movie.Title, movie.Genre, $"{movie.Rating:F2}"));
            }

            context.Movies.AddRange(movies);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;

        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var hallsDto = JsonConvert.DeserializeObject<ImportHallDto[]>(jsonString);

            var sb = new StringBuilder();
            var halls = new List<Hall>();

            foreach (var hallDto in hallsDto)
            {
                if (!IsValid(hallDto) || hallDto.Seats < 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = new Hall
                {
                    Name = hallDto.Name,
                    Is4Dx = hallDto.Is4Dx,
                    Is3D = hallDto.Is3D,
                };

                var seats = new List<Seat>();

                for (int i = 0; i < hallDto.Seats; i++)
                {
                    seats.Add(new Seat
                    {
                        Hall = hall
                    });
                }

                hall.Seats = seats;

                string projectionType = string.Empty;
                var projectionSb = new StringBuilder();

                projectionType = GetProjectionType(projectionSb, hall.Is4Dx, hall.Is3D);

                halls.Add(hall);
                sb.AppendLine(String.Format(SuccessfulImportHallSeat, hall.Name, projectionType, hall.Seats.Count));
            }

            context.Halls.AddRange(halls);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }

        private static string GetProjectionType(StringBuilder projectionSb, bool is4Dx, bool is3D)
        {
            if (!is4Dx && !is3D)
            {
                projectionSb.Append("Normal");
            }
            else if (is4Dx && is3D)
            {
                projectionSb.Append("4Dx/3D");
            }
            else if (is4Dx)
            {
                projectionSb.Append("4Dx");
            }
            else
            {
                projectionSb.Append("3D");
            }

            return projectionSb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportProjectionDto[]), new XmlRootAttribute("Projections"));
            var projectionsDto = (ImportProjectionDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var projections = new List<Projection>();

            foreach (var projectionDto in projectionsDto)
            {
                if (!IsValid(projectionDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var isValidMovie = context.Movies.FirstOrDefault(m => m.Id == projectionDto.MovieId);
                var isValidHall = context.Halls.FirstOrDefault(h => h.Id == projectionDto.HallId);

                if (isValidMovie == null || isValidHall == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var projection = new Projection
                {
                    MovieId = projectionDto.MovieId,
                    HallId = projectionDto.HallId,
                    DateTime = DateTime.ParseExact(projectionDto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };

                projections.Add(projection);
                sb.AppendLine(String.Format(SuccessfulImportProjection, isValidMovie.Title, projection.DateTime.ToString("MM/dd/yyyy")));
            }

            context.Projections.AddRange(projections);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));
            var customersDto = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var sb = new StringBuilder();
            var customers = new List<Customer>();

            foreach (var customerDto in customersDto)
            {
                if (!IsValid(customerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var allTicketsValid = true;

                foreach (var ticketDto in customerDto.Tickets)
                {
                    if (!IsValid(ticketDto))
                    {
                        allTicketsValid = false;
                        break;
                    }
                }

                if (!allTicketsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var customer = new Customer
                {
                    FirstName = customerDto.FirstName,
                    LastName = customerDto.LastName,
                    Age = customerDto.Age,
                    Balance = customerDto.Balance,
                };

                var tickets = new List<Ticket>();

                foreach (var ticket in customerDto.Tickets)
                {
                    tickets.Add(new Ticket
                    {
                        ProjectionId = ticket.ProjectionId,
                        Price = ticket.Price,
                        Customer = customer
                    });
                }

                customer.Tickets = tickets;

                customers.Add(customer);
                sb.AppendLine(String.Format(SuccessfulImportCustomerTicket,customer.FirstName,customer.LastName,customer.Tickets.Count));
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResult, true);

            return isValid;
        }
    }
}