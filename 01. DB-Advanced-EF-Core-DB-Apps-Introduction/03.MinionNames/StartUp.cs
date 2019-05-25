using System;
using System.Data.SqlClient;

namespace _03.MinionNames
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var villianNameQuery = "SELECT Name FROM Villains WHERE Id = @id";

                using (SqlCommand command = new SqlCommand(villianNameQuery,connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    string villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine($"No villain with ID {id} exists in the database.");
                        return;
                    }

                    Console.WriteLine($"Villian: {villainName}");
                }

                string minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                        FROM MinionsVillains AS mv
                                        JOIN Minions As m ON mv.MinionId = m.Id
                                        WHERE mv.VillainId = @Id
                                        ORDER BY m.Name";

                using (SqlCommand command = new SqlCommand(minionsQuery,connection))
                {
                    command.Parameters.AddWithValue(@"Id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            long rowNumber = (long)reader[0];
                            string name = (string)reader[1];
                            int age = (int)reader[2];

                            Console.WriteLine($"{rowNumber}. {name} {age}");
                        }

                        if (!reader.HasRows)
                        {
                            Console.WriteLine("(no minions)");
                        }
                    }

                }
            }
        }
    }
}
