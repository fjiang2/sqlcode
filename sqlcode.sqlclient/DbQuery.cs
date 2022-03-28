using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
	public class DbQuery : DataQuery
	{
		public DbQuery(string connectionString)
			: this(new SqlConnectionStringBuilder(connectionString))
		{
		}

		public DbQuery(SqlConnectionStringBuilder connectionString)
			: this(new SqlDbAgent(connectionString))
		{
		}

		public DbQuery(SqlDbAgent agent)
			: base(agent)
		{
		}
	}
}
