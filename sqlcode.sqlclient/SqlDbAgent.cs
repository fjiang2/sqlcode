using Sys.Data.Entity;

#if NET8_0
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Sys.Data.SqlClient
{
	public class SqlDbAgent : DbAgent
	{
		public SqlDbAgent(SqlConnectionStringBuilder connectionString)
			: base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public override IDbAccess Access(SqlUnit unit) => new SqlDbAccess(ConnectionString.ConnectionString, unit);

	}
}
