using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
	public class SqlDbAgent
		: DbAgent
	{
		private readonly SqlConnectionStringBuilder connectionString;

		public SqlDbAgent(string connectionString)
		{
			this.connectionString = new SqlConnectionStringBuilder(connectionString);
		}

		public SqlDbAgent(SqlConnectionStringBuilder connectionString)
		{
			this.connectionString = connectionString;
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public override IDbAccess Proxy(SqlUnit unit) => new SqlDbAccess(connectionString, unit);
		public override DbAccess Unit(string query, object args) => new SqlDbAccess(connectionString, new SqlUnit(query, args));

		public static DataQuery Query(string connectionString)
			=> new SqlDbAgent(new SqlConnectionStringBuilder(connectionString)).Query();

		public static DataContext Context(string connectionString)
			=> new SqlDbAgent(new SqlConnectionStringBuilder(connectionString)).Context();

	}
}
