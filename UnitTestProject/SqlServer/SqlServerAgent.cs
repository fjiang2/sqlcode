using System.Data.SqlClient;

namespace Sys.Data
{
	public class SqlServerAgent : IDbAgent
	{
		public string ConnectionString { get; }

		public SqlServerAgent(string connectionStirng)
		{
			this.ConnectionString = connectionStirng;
		}

		public IDbCmd Command(DbCmdParameter parameter)
			=> new SqlCmd(new SqlConnectionStringBuilder(ConnectionString), parameter);

		public DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public DbCmdFunction Function => Command;

	}
}
