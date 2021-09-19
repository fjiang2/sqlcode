using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
	public class SqlDbAgent
		: DbAgent
	{
		private SqlConnectionStringBuilder connectionString;

		public SqlDbAgent(SqlConnectionStringBuilder connectionString)
		{
			this.connectionString = connectionString;
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public override IDbAccess Proxy(SqlUnit unit) => new SqlDbAccess(connectionString, unit);

		public static DataQuery Query(string connectionString) 
			=> new DataQuery(new SqlDbAgent(new SqlConnectionStringBuilder(connectionString)));
	}
}
