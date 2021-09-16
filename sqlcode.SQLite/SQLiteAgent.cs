using System.Data.SQLite;
using Sys.Data.Entity;

namespace Sys.Data.SQLite
{
	public class SQLiteAgent : IDbAgent
	{
		private SQLiteConnectionStringBuilder connectionString;

		public SQLiteAgent(string fileName)
		{
			this.connectionString = new SQLiteConnectionStringBuilder($"provider=sqlite;Data Source={fileName};Version=3; DateTimeFormat=Ticks; Pooling=True; Max Pool Size=100;");
		}

		public SQLiteAgent(SQLiteConnectionStringBuilder connectionString)
		{
			this.connectionString = connectionString;
		}

		public DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SQLite };
		public IDbCmd Proxy(SqlUnit unit) => new SQLiteCmd(connectionString, unit);

		public static Query Query(string connectionString)
			=> new Query(new SQLiteAgent(new SQLiteConnectionStringBuilder(connectionString)));
	}
}
