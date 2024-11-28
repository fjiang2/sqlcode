using Sys.Data.SqlRemote;
using Sys.Data.SqlClient;
using Sys.Data.SQLite;
using SqlProxyService.Settings;
using Azure.Core;
using System.Configuration.Provider;

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
            DbAgent? agent = CreateDbAgent(request.Provider);
            if (agent == null)
                return new SqlRemoteResult { Error = $"Cannot find provider or name: {request.Provider}" };

            SqlRemoteHandler handler = new SqlRemoteHandler(agent);
            return handler.Execute(request);
        }

        private DbAgent? CreateDbAgent(DbProvider dbProvider)
        {
            DbServerInfo? serverInfo;
            if (!string.IsNullOrEmpty(dbProvider.Name))
                serverInfo = dbServers.FirstOrDefault(x => x.Name == dbProvider.Name);
            else
                serverInfo = dbServers.FirstOrDefault(x => x.Style == dbProvider.Style);

            if (serverInfo == null)
                return null;

            DbAgent? agent = null;
            switch (serverInfo.Style)
            {
                case DbAgentStyle.SQLite:
                    string fileName = serverInfo.ConnectionString;
                    agent = new SQLiteAgent(fileName);
                    break;

                case DbAgentStyle.SqlServer:
                    agent = new SqlDbAgent(new SqlConnectionStringBuilder(serverInfo.ConnectionString));
                    break;
            }

            return agent;
        }
    }
}