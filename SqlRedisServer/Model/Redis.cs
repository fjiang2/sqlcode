using StackExchange.Redis;

namespace Sys.Data.SqlRedis
{
    public class Redis


    {
        public static ConnectionMultiplexer Manager;

        protected const string Channel = "sql/command";
        public Redis(string connectionString)
        {
            Manager = GetManager(connectionString);
        }

        protected static ConnectionMultiplexer GetManager(string connectionString)
        {
            return ConnectionMultiplexer.Connect(connectionString);
        }

    }
}
