using System.Data.SqlServerCe;
using Sys.Data.Entity;

namespace Sys.Data.SqlCe
{
	public class SqlCeAgent : IDbAgent
	{
		private SqlCeConnectionStringBuilder connectionString;

		public SqlCeAgent(string fileName)
		{
			this.connectionString = new SqlCeConnectionStringBuilder($"Data Source={fileName};Max Buffer Size=1024;Persist Security Info=False;");
		}

		public SqlCeAgent(SqlCeConnectionStringBuilder connectionString)
		{
			this.connectionString = connectionString;
		}

		public DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlCe };
		public IDbCmd Proxy(SqlUnit unit) => new SqlCeCmd(connectionString, unit);

		public static Query Query(string connectionString)
			=> new Query(new SqlCeAgent(new SqlCeConnectionStringBuilder(connectionString)));

	}
}
