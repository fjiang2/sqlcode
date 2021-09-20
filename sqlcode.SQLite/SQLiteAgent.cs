using System.Data.SQLite;
using Sys.Data.Entity;

namespace Sys.Data.SQLite
{
	public class SQLiteAgent : DbAgent
	{
		public SQLiteAgent(string fileName)
			: base(new SQLiteConnectionStringBuilder($"provider=sqlite;Data Source={fileName};Version=3; DateTimeFormat=Ticks; Pooling=True; Max Pool Size=100;"))
		{
		}

		public SQLiteAgent(SQLiteConnectionStringBuilder connectionString)
			: base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SQLite };
		public override DbAccess Access(SqlUnit unit) => new SQLiteAccess(ConnectionString.ConnectionString, unit);

	}
}
