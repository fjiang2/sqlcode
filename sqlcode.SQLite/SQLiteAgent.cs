using System.Data.SQLite;
using Sys.Data.Entity;

namespace Sys.Data.SQLite
{
	public class SQLiteAgent : DbAgent
	{
		private readonly SQLiteConnectionStringBuilder connectionString;

		public SQLiteAgent(string fileName)
		{
			this.connectionString = new SQLiteConnectionStringBuilder($"provider=sqlite;Data Source={fileName};Version=3; DateTimeFormat=Ticks; Pooling=True; Max Pool Size=100;");
		}

		public SQLiteAgent(SQLiteConnectionStringBuilder connectionString)
		{
			this.connectionString = connectionString;
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SQLite };
		public override IDbAccess Proxy(SqlUnit unit) => new SQLiteAccess(connectionString, unit);
		public override DbAccess Unit(string query, object args) => new SQLiteAccess(connectionString, new SqlUnit(query, args));

		public static DataQuery Query(string connectionString)
			=> new SQLiteAgent(new SQLiteConnectionStringBuilder(connectionString)).Query();

		public static DataContext Context(string connectionString)
			=> new SQLiteAgent(new SQLiteConnectionStringBuilder(connectionString)).Context();
	}
}
