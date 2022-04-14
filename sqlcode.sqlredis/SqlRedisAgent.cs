using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data.SqlRedis
{
    public class SqlRedisAgent : DbAgent
	{
		public SqlRedisAgent(SqlRedisConnectionStringBuilder connectionString)
			:base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public override IDbAccess Access(SqlUnit unit) => new SqlRedisAccess(ConnectionString.ConnectionString, unit);

	}
}
