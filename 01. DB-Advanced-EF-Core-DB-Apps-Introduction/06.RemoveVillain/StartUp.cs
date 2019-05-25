using System;
using System.Data.SqlClient;

namespace _06.RemoveVillain
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var villainId = int.Parse(Console.ReadLine());

            string villainNameQ = "SELECT Name FROM Villains WHERE Id = {0}";
            string minionsCountQuery = "SELECT COUNT(*) FROM MinionsVillains WHERE VillainId = {0}";
            string deleteFromTable = "DELETE FROM {0} WHERE {1} = {2}";

            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                var command = new SqlCommand(string.Format(minionsCountQuery, villainId), connection, transaction);
                var releasedMinions = (int)command.ExecuteScalar();

                command = new SqlCommand(string.Format(villainNameQ, villainId), connection, transaction);
                var name = (string)command.ExecuteScalar();

                if (name is null)
                {
                    Console.WriteLine("No such villain was found.");
                    transaction.Rollback();
                    return;
                }

                try
                {
                    TryExecuteCommand(connection, transaction, string.Format(deleteFromTable, "MinionsVillains", "VillainId", villainId));
                    TryExecuteCommand(connection, transaction, string.Format(deleteFromTable, "Villains", "Id", villainId));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    transaction.Rollback();
                    return;
                }

                Console.WriteLine($"{name} was deleted.");
                Console.WriteLine(releasedMinions + " minions were released.");

                transaction.Commit();
            }
        }

        public static void TryExecuteCommand(SqlConnection connection, SqlTransaction transaction, string commandString)
        {
            var command = new SqlCommand(commandString, connection, transaction);
            command.ExecuteNonQuery();
        }
    }
}
