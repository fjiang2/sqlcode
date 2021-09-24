using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
	public class DbQuery : DataQuery
	{
		public DbQuery(string connectionString)
			: base(new SqlDbAgent(new SqlConnectionStringBuilder(connectionString)))
		{
		}
	}
}
