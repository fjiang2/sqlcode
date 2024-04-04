using sqlcode.dynamodb.ado;
using Sys.Data.Entity;

namespace Sys.Data.DynamoDb
{
    public class DbQuery : DataQuery
	{
		public DbQuery(string connectionString)
			: this(new DynamoDbConnectionStringBuilder(connectionString))
		{
		}

		public DbQuery(DynamoDbConnectionStringBuilder connectionString)
			: base(new DynamoDbAgent(connectionString))
		{
		}

		public DbQuery(DynamoDbAgent agent)
			: base(agent)
		{
		}
	}
}
