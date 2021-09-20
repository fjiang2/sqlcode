using System.Data.SqlServerCe;
using Sys.Data.Entity;

namespace Sys.Data.SqlCe
{
	public class SqlCeAgent : DbAgent
	{
		private readonly SqlCeConnectionStringBuilder connectionString;

		public SqlCeAgent(string fileName)
		{
			this.connectionString = new SqlCeConnectionStringBuilder($"Data Source={fileName};Max Buffer Size=1024;Persist Security Info=False;");
		}

		public SqlCeAgent(SqlCeConnectionStringBuilder connectionString)
		{
			this.connectionString = connectionString;
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlCe };
		public override IDbAccess Proxy(SqlUnit unit) => new SqlCeAccess(connectionString, unit);
		public override DbAccess Unit(string query, object args) => new SqlCeAccess(connectionString, new SqlUnit(query, args));

		public static DataQuery Query(string connectionString)
			=> new SqlCeAgent(new SqlCeConnectionStringBuilder(connectionString)).Query();

		public static DataContext Context(string connectionString)
			=> new SqlCeAgent(new SqlCeConnectionStringBuilder(connectionString)).Context();


		public void CreateDatabase()
		{
			using (SqlCeEngine engine = new SqlCeEngine(connectionString.ConnectionString))
			{
				engine.CreateDatabase();
			}
		}
	}
}
