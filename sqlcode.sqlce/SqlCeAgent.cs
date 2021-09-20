using System.Data.SqlServerCe;
using Sys.Data.Entity;

namespace Sys.Data.SqlCe
{
	public class SqlCeAgent : DbAgent
	{
		public SqlCeAgent(string fileName)
			: base(new SqlCeConnectionStringBuilder($"Data Source={fileName};Max Buffer Size=1024;Persist Security Info=False;"))
		{
		}

		public SqlCeAgent(SqlCeConnectionStringBuilder connectionString)
			: base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlCe };
		public override DbAccess Access(SqlUnit unit) => new SqlCeAccess(ConnectionString.ConnectionString, unit);

	}
}
