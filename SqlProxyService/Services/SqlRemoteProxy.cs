#if NET8_0
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

using Sys.Data.SqlRemote;
using Sys.Data.SqlClient;
using Sys.Data;
using Sys.Data.SQLite;

namespace SqlProxyService.Services
{
    class SqlRemoteProxy
    {
        private readonly string connectionString;

        public SqlRemoteProxy(string connectionString)
        {
            this.connectionString = connectionString;                
        }

        private static string Now => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        public string Execute(string json)
        {
            var request = Json.Deserialize<SqlRemoteRequest>(json);
            Console.WriteLine($"{Now} [Req] {request}");

            SqlRemoteResult result = Execute(request);

            json = Json.Serialize(result);
            Console.WriteLine($"{Now} [Ret] {result}");

            return json;
        }

        public SqlRemoteResult Execute(SqlRemoteRequest request)
        {
            DbAgent agent = CreateDbAgent(request);

            SqlRemoteHandler handler = new SqlRemoteHandler(agent);
            return handler.Execute(request);
        }

        private DbAgent CreateDbAgent(SqlRemoteRequest request)
        {
            DbAgent agent;
            switch (request.Style)
            {
                case DbAgentStyle.SQLite:
                    string fileName = connectionString;
                    agent = new SQLiteAgent(fileName);
                    break;

                default:
                    agent = new SqlDbAgent(new SqlConnectionStringBuilder(connectionString));
                    break;
            }

            return agent;
        }
    }
}