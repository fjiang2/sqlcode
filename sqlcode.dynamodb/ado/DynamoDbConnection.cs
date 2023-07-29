using System.Data;
using System.Data.Common;

namespace sqlcode.dynamodb.ado
{
    class DynamoDbConnection : DbConnection
    {
        public override string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string Database => throw new NotImplementedException();

        public override string DataSource => throw new NotImplementedException();

        public override string ServerVersion => throw new NotImplementedException();

        public override ConnectionState State => ConnectionState.Closed;


        public DynamoDbConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
        }

        public override void Open()
        {

        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new DynamoDbCommand();
        }
    }
}
