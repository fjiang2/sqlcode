using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
	internal class SqlDbAgent : DbAgent
	{
		public SqlDbAgent(string connectionString)
			: base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public override IDbAccess Access(SqlUnit unit) => new SqlDbAccess(ConnectionString, unit);

	}
}
