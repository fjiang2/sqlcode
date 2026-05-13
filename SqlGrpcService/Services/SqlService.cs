using Grpc.Core;
using Sys.Data.SqlClient;
using Sys.Data.SQLite;
using Sys.Data.SqlRemote;

namespace SqlGrpcService.Services
{
    public class SqlService : SqlApi.SqlApiBase
    {
        private readonly List<DbServerInfo> dbServers;
        private readonly ILogger<SqlService> logger;

        public SqlService(ILogger<SqlService> logger, ISetting setting)
        {
            this.dbServers = setting.ServerOption.DbServers;
            this.logger = logger;
        }

        public override Task<SqlResponse> Execute(SqlRequest request, ServerCallContext context)
        {
            logger.LogInformation("The message is received from {Name}", request.Body);

            Console.WriteLine($"{DateTime.Now} [Req] {request.RequestId} {request.Body}");
            var sqlRequest = Json.Deserialize<SqlRemoteRequest>(request.Body);

            SqlRemoteResult sqlResult = Execute(sqlRequest);
            string json = Json.Serialize(sqlResult);

            Console.WriteLine($"{DateTime.Now} [Ret] {request.RequestId} {json}");

            return Task.FromResult(new SqlResponse
            {
                RequestId = request.RequestId,
                Result = json
            });
        }

        private SqlRemoteResult Execute(SqlRemoteRequest request)
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
            if (!string.IsNullOrEmpty(dbProvider.Name))
                serverInfo = dbServers.FirstOrDefault(x => x.Name == dbProvider.Name);
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
