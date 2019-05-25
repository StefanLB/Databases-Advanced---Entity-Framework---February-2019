using System;
using System.Data.SqlClient;
using System.Linq;

namespace _08.IncreaseMinionAge
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            string updateMinions = "UPDATE Minions SET Name = upper(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1 WHERE Id in ({0})";
            string getUpdatedValues = "SELECT Name, Age FROM Minions WHERE Id in ({0})";

            var minionsToUpdate = Console.ReadLine()
                                        .Split()
                                        .Select(int.Parse)
                                        .ToArray();

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(string.Format(updateMinions, string.Join(", ", minionsToUpdate)), connection))
                {
                    var rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Print("No minions found with the provided ids!");
                        return;
                    }
                    Print("Rows Affected: " + rowsAffected);
                }

                using (var command = new SqlCommand(string.Format(getUpdatedValues, string.Join(", ", minionsToUpdate)), connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Print(reader[0] + " " + reader[1]);
                        }
                    }
                }
            }
        }

        static void Print(string text) => Console.WriteLine(text);
    }
}
