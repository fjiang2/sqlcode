using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Sys.Data.SqlRedis
{
    class Program
    {
        public const string connectionString = "Server = (LocalDB)\\MSSQLLocalDB;initial catalog=Northwind;Integrated Security = true;";

        private static void Main()
        {
            var options = new ConfigurationOptions
            {
                EndPoints = { "localhost:6379" }
            };

            Console.WriteLine("Listening Redis ....");
            var server = new SqlRedisServer(options, new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString));
            Console.ReadKey();
        }
    }
}
