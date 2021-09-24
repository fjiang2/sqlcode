using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
	public class SqlDbAgent : DbAgent
	{
		public SqlDbAgent(SqlConnectionStringBuilder connectionString)
			: base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public override DbAccess Access(SqlUnit unit) => new SqlDbAccess(ConnectionString.ConnectionString, unit);

	}
}
