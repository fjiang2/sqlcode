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

		public IDbCmd Command(string sql, object args)
			=> new SqlCmd(new SqlConnectionStringBuilder(ConnectionString), sql, args);

		public DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public DbCmdFunction Function => Command;

	}
}
