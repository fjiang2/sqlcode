using Sys.Data.Entity;

namespace Sys.Data.DynamoDb
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

		public DbQuery(DynamoDbAgent agent)
			: base(agent)
		{
		}
	}
}
