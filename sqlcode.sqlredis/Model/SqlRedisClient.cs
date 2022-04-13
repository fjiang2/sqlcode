using System;
using System.Threading.Tasks;
using Sys.Data.SqlRemote;
using StackExchange.Redis;

namespace Sys.Data.SqlRedis
{

    public class SqlRedisClient : RedisClient, ISqlMessageClient
    {
        public SqlRedisClient(string connectionString)
            :base(connectionString)
        {
        }


        public Task<SqlResultMessage> RequesteAsync(SqlRequestMessage request)
        {
            string json = Json.Serialize(request);
            ISubscriber sub = Manager.GetSubscriber();
            var x = sub.Publish(Channel, json);

            SqlResultMessage result = new SqlResultMessage();

            return new Task<SqlResultMessage>(() => result);
        }

    }
}
