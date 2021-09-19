using System.Data.SQLite;
using Sys.Data.Entity;

namespace Sys.Data.SQLite
{
	public class SQLiteAgent : DbAgent
	{
		private SQLiteConnectionStringBuilder connectionString;

		public SQLiteAgent(SQLiteConnectionStringBuilder connectionString)
		{
			this.connectionString = connectionString;
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SQLite };
		public override IDbAccess Proxy(SqlUnit unit) => new SQLiteAccess(connectionString, unit);

		public static DataQuery Query(string connectionString)
			=> new DataQuery(new SQLiteAgent(new SQLiteConnectionStringBuilder(connectionString)));
	}
}
