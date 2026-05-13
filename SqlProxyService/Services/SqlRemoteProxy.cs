using Sys.Data.SqlRemote;
using Sys.Data.SqlClient;
using Sys.Data.SQLite;
using SqlProxy.Service.Settings;

namespace SqlProxy.Service.Services
{
    class SqlRemoteProxy
    {
        private readonly List<DbServerInfo> dbServers;

        public SqlRemoteProxy(ServerOption option)
        {
            this.dbServers = option.DbServers;
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
            IDbAgent? agent = CreateDbAgent(request.Provider);
            if (agent == null)
                return new SqlRemoteResult 
                { 
                    Error = $"Cannot find provider or name: {request.Provider}" 
                };

            SqlRemoteHandler handler = new SqlRemoteHandler(agent);
            return handler.Execute(request);
        }

        private IDbAgent? CreateDbAgent(DbProvider dbProvider)
        {
            DbServerInfo? serverInfo;
            if (!string.IsNullOrEmpty(dbProvider.ServerName))
                serverInfo = dbServers.FirstOrDefault(x => x.Name == dbProvider.ServerName);
            else
                serverInfo = dbServers.FirstOrDefault(x => x.Style == dbProvider.Style);

            if (serverInfo == null)
                return null;

            IDbAgent? agent = null;
            switch (serverInfo.Style)
            {
                case DbAgentStyle.SQLite:
                    agent = new SQLiteClient(serverInfo.ConnectionString).Agent;
                    break;

                case DbAgentStyle.SqlServer:
                    agent = new SqlDbClient(serverInfo.ConnectionString).Agent;
                    break;
            }

            return agent;
        }
    }
}