using Amazon.Runtime;
using sqlcode.dynamodb.ado;
using Sys.Data.Entity;

namespace Sys.Data.DynamoDb
{
    public class DynamoDbAgent : DbAgent
	{
		public DynamoDbAgent(DynamoDbConnectionStringBuilder connectionString)
			: base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public override IDbAccess Access(SqlUnit unit) => new DynamoDbAccess(ConnectionString.ConnectionString, unit);

	}
}
