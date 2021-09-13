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

		public DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public IDbCmd Function(DbCmdParameter parameter) => new SqlCmd(new SqlConnectionStringBuilder(ConnectionString), parameter);


	}
}
