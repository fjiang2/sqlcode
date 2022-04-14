using System;

namespace sqlcode.sqlweb
{
    class Program
    {
        private static void Main()
        {
            string url = "http://localhost/sqlhandler/";
            Console.WriteLine($"Listen: {url}");
            var ws = new SqlHttpServer(url);
            ws.Start();
            Console.ReadKey();
            ws.Stop();
        }
    }
}
