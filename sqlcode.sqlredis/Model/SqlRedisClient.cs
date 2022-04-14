using System;
using System.Threading.Tasks;
using Sys.Data.SqlRemote;
using StackExchange.Redis;

namespace Sys.Data.SqlRedis
{

    public class SqlRedisClient : RedisClient, ISqlRemoteClient
    {
        public SqlRedisClient(string connectionString)
            :base(connectionString)
        {
        }


        public Task<SqlRemoteResult> RequesteAsync(SqlRemoteRequest request)
        {
            string json = Json.Serialize(request);
            ISubscriber sub = Manager.GetSubscriber();
            var x = sub.Publish(Channel, json);

            SqlRemoteResult result = new SqlRemoteResult();

            return new Task<SqlRemoteResult>(() => result);
        }

    }
}
