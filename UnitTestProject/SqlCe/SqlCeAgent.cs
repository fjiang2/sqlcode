using System.Data.SqlServerCe;

namespace Sys.Data
{
	public class SqlCeAgent : IDbAgent
	{
		private string fileName;

		public SqlCeAgent(string fileName)
		{
			this.fileName = fileName;
		}

		public string ConnectionString
			=> $"Data Source={fileName};Max Buffer Size=1024;Persist Security Info=False;";

		public DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlCe };
		public IDbCmd Proxy(SqlUnit unit) => new SqlCeCmd(new SqlCeConnectionStringBuilder(ConnectionString), unit);

	}
}
