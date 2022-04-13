using System.Data;
using Sys.Data.Entity;

namespace Sys.Data.SqlRedis
{
	public class DbQuery : DataQuery
	{
		public DbQuery(string connectionString)
			: this(new SqlRedisConnectionStringBuilder(connectionString))
		{
		}

		public DbQuery(SqlRedisConnectionStringBuilder connectionString)
			: base(new SqlRedisAgent(connectionString))
		{
		}

		public DbQuery(SqlRedisAgent agent)
			: base(agent)
		{
		}
	}
}
