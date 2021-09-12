using System.Data.SQLite;

namespace Sys.Data
{
	public class SQLiteAgent : IDbAgent
	{
		private string fileName;

		public SQLiteAgent(string fileName)
		{
			this.fileName = fileName;
		}

		public string ConnectionString
			=> $"provider=sqlite;Data Source={fileName};Version=3; DateTimeFormat=Ticks; Pooling=True; Max Pool Size=100;";

		public IDbCmd Command(string sql, object args)
			=> new SQLiteCmd(new SQLiteConnectionStringBuilder(ConnectionString), sql, args);

		public DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SQLite };
		public DbCmdFunction Function => Command;

	}
}
