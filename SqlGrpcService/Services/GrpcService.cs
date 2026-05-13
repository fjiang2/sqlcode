using System.Text.Json;
using Grpc.Core;
using Sys.Data.SqlClient;
using Sys.Data.SQLite;
using Sys.Data.SqlRemote;

namespace SqlGrpcService.Services
{
    public class GrpcService : SqlApi.SqlApiBase
    {
        private readonly List<DbServerInfo> dbServers;
        private readonly ILogger<GrpcService> logger;

        public GrpcService(ILogger<GrpcService> logger, ISetting setting)
        {
            this.dbServers = setting.ServerOption.DbServers;
            this.logger = logger;
        }

        private static string Now => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        public override Task<SqlResponse> Execute(SqlRequest request, ServerCallContext context)
        {
            //logger.LogInformation("The message is received from {Name}", request.Body);

            SqlRemoteRequest sqlRequest = Json.ToSqlRemoteRequest(request.Body);
            Console.WriteLine($"{Now} [Req] {request.RequestId} {sqlRequest}");

            SqlRemoteResult sqlResult = Execute(sqlRequest);
            Console.WriteLine($"{Now} [Ret] {request.RequestId} {sqlResult}");
            
            string json = Json.Serialize(sqlResult);
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
