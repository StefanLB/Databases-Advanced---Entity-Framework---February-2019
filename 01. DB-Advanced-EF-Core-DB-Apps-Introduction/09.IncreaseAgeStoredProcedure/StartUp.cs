using System;
using System.Data;
using System.Data.SqlClient;

namespace _09.IncreaseAgeStoredProcedure
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            string procedureName = "usp_GetOlder";
            string updatedMinion = "SELECT Name, Age FROM Minions WHERE Id = {0}";


            var id = int.Parse(Console.ReadLine());

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected != 1)
                    {
                        Print($"No minion with id {id} found!");
                        return;
                    }
                }

                using (var command = new SqlCommand(string.Format(updatedMinion, id), connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        Print(reader[0] + " - " + reader[1] + " years old");
                    }
                }
            }
        }

        static void Print(string text) => Console.WriteLine(text);

    }
}
