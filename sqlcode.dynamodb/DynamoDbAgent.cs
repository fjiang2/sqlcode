//using Amazon.Runtime;
//using Sys.Data.Entity;

//namespace Sys.Data.DynamoDb
//{
//	public class DynamoDbAgent : DbAgent
//	{
//		public DynamoDbAgent(AWSCredentials connectionString)
//			: base(connectionString)
//		{
//		}

//		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
//		public override IDbAccess Access(SqlUnit unit) => new DynamoDbAccess(ConnectionString.ConnectionString, unit);

//	}
//}
