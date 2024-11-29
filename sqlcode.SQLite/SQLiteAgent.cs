using System.Data.SQLite;
using Sys.Data.Entity;

namespace Sys.Data.SQLite
{
    internal class SQLiteAgent : DbAgent
	{
		public SQLiteAgent(SQLiteConnectionStringBuilder connectionString)
			: base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SQLite };
		public override IDbAccess Access(SqlUnit unit) => new SQLiteAccess(ConnectionString.ConnectionString, unit);

	}
}
