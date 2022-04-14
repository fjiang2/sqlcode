using System;
using System.Data.SqlClient;

using Sys.Data.SqlRemote;
using Sys.Data.SqlClient;
using StackExchange.Redis;

namespace Sys.Data.SqlRedis
{
    public class SqlRedisServer : RedisClient, ISqlRemoteServer
    {
        private SqlRemoteHandler handler;

        public SqlRedisServer(string connectionString, SqlConnectionStringBuilder connection)
            :base(connectionString)
        {
            handler = new SqlRemoteHandler(new SqlDbAgent(connection));
        }

        public SqlRemoteResult Execute(SqlRemoteRequest request)
        {
            SqlRemoteResult result = handler.Process(request);
            string json = Json.Serialize(result);

            ISubscriber sub = Manager.GetSubscriber();
            var x = sub.Publish(Channel, json);
            return null;
        }
    }
}
