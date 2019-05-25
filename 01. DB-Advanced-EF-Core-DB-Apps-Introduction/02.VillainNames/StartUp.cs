using System;
using System.Data.SqlClient;

namespace _02.VillainNames
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var query = @"  SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                FROM Villains AS v 
                                JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                                GROUP BY v.Id, v.Name 
                                HAVING COUNT(mv.VillainId) > 3 
                                ORDER BY COUNT(mv.VillainId)";

                using (var command = new SqlCommand(query, connection))
                {
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var villain = (string)reader["Name"];
                        var minionsCount = (int)reader["MinionsCount"];


                        Console.WriteLine(villain + " - " + minionsCount);
                    }
                }
            }
        }
    }
}
