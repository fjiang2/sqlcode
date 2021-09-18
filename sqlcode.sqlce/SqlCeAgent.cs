using System.Data.SqlServerCe;
using Sys.Data.Entity;

namespace Sys.Data.SqlCe
{
	public class SqlCeAgent : DbAgent
	{
		private SqlCeConnectionStringBuilder connectionString;

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

		public void CreateDatabase()
		{
			using (SqlCeEngine engine = new SqlCeEngine(connectionString.ConnectionString))
			{
				engine.CreateDatabase();
			}
		}

		public static DataQuery Query(string connectionString)
			=> new DataQuery(new SqlCeAgent(new SqlCeConnectionStringBuilder(connectionString)));


	}
}
