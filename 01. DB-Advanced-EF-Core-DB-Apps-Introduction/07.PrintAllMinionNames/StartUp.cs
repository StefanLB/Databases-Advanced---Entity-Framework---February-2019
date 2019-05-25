using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _07.PrintAllMinionNames
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            string getMinionsNames = "SELECT Name FROM Minions";


            var minionsNames = new List<string>();
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(getMinionsNames, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            minionsNames.Add((string)reader[0]);
                        }
                    }
                }
            }

            var first = 0;
            var last = minionsNames.Count - 1;
            for (int i = 0; i < minionsNames.Count / 2; i++)
            {
                Console.WriteLine(minionsNames[first++]);
                Console.WriteLine(minionsNames[last--]);
            }

            if (minionsNames.Count % 2 != 0 && first == minionsNames.Count / 2)
            {
                Console.WriteLine(minionsNames[first]);
            }
        }
    }
}
