using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _05.ChangeTownNamesCasing
{
    public class StartUp
    {
        public static void Main(string[] args)
        {

            string townsInCountry = "SELECT t.Name FROM Towns as t JOIN Countries AS c ON c.Id = t.CountryCode WHERE c.Name = '{0}'";
            string townsToUpper = "UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = '{0}')";

            var country = Console.ReadLine();

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var transaction = connection.BeginTransaction();
                var reader = ReaderGetTowns(townsInCountry, country, connection, transaction);

                if (!reader.HasRows)
                {
                    Console.WriteLine("No town names were affected.");
                    reader.Close();
                    transaction.Rollback();
                    return;
                }
                reader.Close();

                var command = new SqlCommand(string.Format(townsToUpper, country), connection, transaction);
                var rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " town names were affected.");

                reader = ReaderGetTowns(townsInCountry, country, connection, transaction);
                var towns = new List<string>();
                while (reader.Read())
                {
                    towns.Add((string)reader[0]);
                }

                Console.WriteLine("[" + string.Join(", ", towns) + "]");
                reader.Close();

                transaction.Commit();
            }
        }

        static SqlDataReader ReaderGetTowns(string townsInCountry, string country, SqlConnection connection, SqlTransaction tran)
        {
            var command = new SqlCommand(string.Format(townsInCountry, country), connection, tran);
            var reader = command.ExecuteReader();

            return reader;
        }
    }
}
