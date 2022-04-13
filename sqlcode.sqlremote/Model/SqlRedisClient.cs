using System;
using System.Threading.Tasks;
using Sys.Data.SqlRemote;

namespace Sys.Data.SqlRedis
{
    public class SqlRedisClient: ISqlMessageClient
    {
        private readonly string connectinString;

        public SqlRedisClient(string connectionString)
        {
            this.connectinString = connectionString;
        }

        public Task<SqlResultMessage> RequesteAsync(SqlRequestMessage request)
        {
            SqlResultMessage result = new SqlResultMessage();
         

            var connection = new SqlRedisConnectionStringBuilder(connectinString);

            return new Task<SqlResultMessage>(() => result);
        }
    }
}
