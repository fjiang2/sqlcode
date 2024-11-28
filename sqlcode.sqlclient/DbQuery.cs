using Sys.Data.Entity;

#if NET8_0
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Sys.Data.SqlClient
{
	public class DbQuery : DataQuery
	{
		public DbQuery(string connectionString)
			: this(new SqlConnectionStringBuilder(connectionString))
		{
		}

		public DbQuery(SqlConnectionStringBuilder connectionString)
			: base(new SqlDbAgent(connectionString))
		{
		}

		public DbQuery(SqlDbAgent agent)
			: base(agent)
		{
		}
	}
}
