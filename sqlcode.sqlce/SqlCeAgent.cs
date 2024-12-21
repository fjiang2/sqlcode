using System.Data.SqlServerCe;
using Sys.Data.Entity;

namespace Sys.Data.SqlCe
{
	internal class SqlCeAgent : DbAgent
	{
	
		public SqlCeAgent(string connectionString)
			: base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlCe };
		public override IDbAccess Access(SqlUnit unit) => new SqlCeAccess(ConnectionString, unit);

	}
}
