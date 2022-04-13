using System;
using System.Data.SqlClient;

using Sys.Data.SqlRemote;
using Sys.Data.SqlClient;
using StackExchange.Redis;

namespace Sys.Data.SqlRedis
{
    public class SqlRedisServer : RedisClient, ISqlMessageServer
    {
        private SqlMessageProcessor processor;

        public SqlRedisServer(string connectionString, SqlConnectionStringBuilder connection)
            :base(connectionString)
        {
            processor = new SqlMessageProcessor(new SqlDbAgent(connection));
        }

        public void OnRequest(SqlRequestMessage request)
        {
            SqlResultMessage result = processor.Process(request);
            string json = Json.Serialize(result);

            ISubscriber sub = Manager.GetSubscriber();
            var x = sub.Publish(Channel, json);
        }
    }
}
