using System;

namespace sqlcode.sqlweb
{
    class Program
    {
        public const string url = "http://localhost/sqlhandler/";
        public const string connectionString = "Server = (LocalDB)\\MSSQLLocalDB;initial catalog=Northwind;Integrated Security = true;";

        private static void Main()
        {
            Console.WriteLine($"Listen: {url}");
            var ws = new SqlHttpServer(url);
            ws.Start();
            Console.ReadKey();
            ws.Stop();
        }
    }
}
