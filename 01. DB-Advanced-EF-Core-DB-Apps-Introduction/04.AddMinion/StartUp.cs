using System;
using System.Data.SqlClient;

namespace _04.AddMinion
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var minionTokens = Console.ReadLine().Split();
            var villainName = Console.ReadLine().Split()[1];

            var minionName = minionTokens[1];
            var age = int.Parse(minionTokens[2]);
            var town = minionTokens[3];

            string getTown = "SELECT Id FROM Towns WHERE Name = '{0}'";
            string getVillain = "SELECT Id FROM Villains WHERE Name = '{0}'";
            string getMinion = "SELECT Id FROM Minions WHERE Name = '{0}'";
            string insertTown = "INSERT INTO Towns (Name) VALUES ('{0}')";
            string insertVillain = "INSERT INTO Villains (Name, EvilnessFactorId) VALUES ('{0}', 4)";
            string insertMinion = "INSERT INTO Minions (Name, Age, TownId) VALUES ('{0}', {1}, {2})";
            string insertMinionAndVillain = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES ({0}, {1})";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var townId = ExecuteScalar(string.Format(getTown, town), connection);
                if (townId is null)
                {
                    ExecuteNonQuery(string.Format(insertTown, town), connection);
                    Print($"Town {town} was added to the database.");
                    townId = ExecuteScalar(string.Format(getTown, town), connection);
                }

                var villainId = ExecuteScalar(string.Format(getVillain, villainName), connection);
                if (villainId is null)
                {
                    ExecuteNonQuery(string.Format(insertVillain, villainName), connection);
                    villainId = ExecuteScalar(string.Format(getVillain, villainName), connection);
                    Print($"Villain {villainName} was added to the database.");
                }

                ExecuteNonQuery(string.Format(insertMinion, minionName, age, townId), connection);
                var minionId = ExecuteScalar(string.Format(getMinion, minionName), connection);
                ExecuteNonQuery(string.Format(insertMinionAndVillain, minionId, villainId), connection);

                Print($"Successfully added {minionName} to be minion of {villainName}.");

            }
        }

        static void Print(string text) => Console.WriteLine(text);

        static object ExecuteScalar(string query, SqlConnection connection)
        {
            return new SqlCommand(query, connection).ExecuteScalar();
        }

        static int ExecuteNonQuery(string query, SqlConnection connection)
        {
            return new SqlCommand(query, connection).ExecuteNonQuery();
        }
    }
}
