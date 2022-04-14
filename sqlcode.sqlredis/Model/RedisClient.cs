using StackExchange.Redis;

namespace Sys.Data.SqlRedis
{
    public class RedisClient

    {
        public static ConnectionMultiplexer Manager;

        protected const string Channel = "sql/command";
        public RedisClient(string connectionString)
        {
            Manager = GetManager(connectionString);
        }

        protected static ConnectionMultiplexer GetManager(string connectionString)
        {
            return ConnectionMultiplexer.Connect(connectionString);
        }

    }
}
