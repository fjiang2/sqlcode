using System.Data.SqlClient;

namespace Sys.Data.SqlClient
{
	public class SqlClientAgent : IDbAgent
	{
		public string ConnectionString { get; }

		public SqlClientAgent(string connectionStirng)
		{
			this.ConnectionString = connectionStirng;
		}

		public DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public IDbCmd Proxy(SqlUnit unit) => new SqlCmd(new SqlConnectionStringBuilder(ConnectionString), unit);


	}
}
