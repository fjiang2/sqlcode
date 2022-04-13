using System;
using System.Threading.Tasks;
using Sys.Data.SqlRemote;
using StackExchange.Redis;

namespace Sys.Data.SqlRedis
{
    public class SqlRedisClient : ISqlMessageClient
    {
        private readonly string connectinString;
        public static ConnectionMultiplexer Manager;

        public SqlRedisClient(string connectionString)
        {
            this.connectinString = connectionString;
            Manager = GetManager(connectionString);
        }

        private static ConnectionMultiplexer GetManager(string connectionString)
        {
            return ConnectionMultiplexer.Connect(connectionString);
        }


        public Task<SqlResultMessage> RequesteAsync(SqlRequestMessage request)
        {
            string json = Json.Serialize(request);
            ISubscriber sub = Manager.GetSubscriber();
            var x = sub.Publish(request.RequestId.ToByteArray(), json);

            SqlResultMessage result = new SqlResultMessage();
            var connection = new SqlRedisConnectionStringBuilder(connectinString);

            return new Task<SqlResultMessage>(() => result);
        }

    }

    public class SqlRedisServer : ISqlMessageServer
    {
        public void OnRequest(SqlRequestMessage request)
        {
            throw new NotImplementedException();
        }
    }
}
