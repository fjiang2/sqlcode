
using System.Data.SQLite;
using Sys.Data.Entity;

namespace Sys.Data.SQLite
{
	public class DbQuery : DataQuery
	{
		public DbQuery(string connectionString)
			: base(new SQLiteAgent(new SQLiteConnectionStringBuilder(connectionString)))
		{
		}
	}
}
