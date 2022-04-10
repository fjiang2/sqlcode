using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteAgent : DbAgent
	{
		public SqlRemoteAgent(SqlRemoteConnectionStringBuilder connectionString)
			: base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public override IDbAccess Access(SqlUnit unit) => new SqlRemoteAccess(ConnectionString.ConnectionString, unit);

	}
}
