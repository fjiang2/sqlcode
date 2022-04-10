using System.Data;
using Sys.Data.Entity;

namespace Sys.Data.SqlRemote
{
	public class DbQuery : DataQuery
	{
		public DbQuery(string connectionString)
			: this(new SqlRemoteConnectionStringBuilder(connectionString))
		{
		}

		public DbQuery(SqlRemoteConnectionStringBuilder connectionString)
			: base(new SqlRemoteAgent(connectionString))
		{
		}

		public DbQuery(SqlRemoteAgent agent)
			: base(agent)
		{
		}
	}
}
