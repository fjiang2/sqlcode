
using System.Data.SqlServerCe;
using Sys.Data.Entity;

namespace Sys.Data.SqlCe
{
	public class DbQuery : DataQuery
	{
		public DbQuery(string connectionString)
			: base(new SqlCeAgent(new SqlCeConnectionStringBuilder(connectionString)))
		{
		}
	}
}
