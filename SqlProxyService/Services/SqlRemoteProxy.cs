using Sys.Data.SqlRemote;
using Sys.Data.SqlClient;
using Sys.Data.SQLite;
using SqlProxyService.Settings;

namespace SqlProxyService.Services
{
    class SqlRemoteProxy
    {
        private readonly List<DbServerInfo> dbServers;

        public SqlRemoteProxy(List<DbServerInfo> dbServers)
        {
            this.dbServers = dbServers;
        }

        private static string Now => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        public string Execute(string json)
        {
            var request = Json.Deserialize<SqlRemoteRequest>(json);
            Console.WriteLine($"{Now} [Tx] {request}");

            SqlRemoteResult result = Execute(request);

            json = Json.Serialize(result);
            Console.WriteLine($"{Now} [Rx] {result}");

            return json;
        }

        public SqlRemoteResult Execute(SqlRemoteRequest request)
        {
            string providerName = request.Provider.Name;
            DbServerInfo serverInfo = dbServers.Where(x => x.Name == providerName).First();
            if (serverInfo == null)
            {
                return new SqlRemoteResult { Error = $"Cannot find provider name: {providerName}" };
            }

            DbAgent agent = CreateDbAgent(serverInfo);

            SqlRemoteHandler handler = new SqlRemoteHandler(agent);
            return handler.Execute(request);
        }

        private DbAgent CreateDbAgent(DbServerInfo serverInfo)
        {
            DbAgent agent;
            switch (serverInfo.Style)
            {
                case DbAgentStyle.SQLite:
                    string fileName = serverInfo.ConnectionString;
                    agent = new SQLiteAgent(fileName);
                    break;

                default:
                    agent = new SqlDbAgent(new SqlConnectionStringBuilder(serverInfo.ConnectionString));
                    break;
            }

            return agent;
        }
    }
}