using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Amazon.Lambda;

namespace sqlcode.dynamodb.ado
{
    class DynamoDbConnection : DbConnection
    {
        [AllowNull]
        public override string ConnectionString { get; set; }

        public override string Database => database;

        public override string DataSource => datasource;

        public override string ServerVersion => string.Empty;

        public override ConnectionState State => state;


        private string database;
        private string datasource;
        private ConnectionState state = ConnectionState.Closed;

        public DynamoDbConnectionStringBuilder ConnecitonStingBuilder { get; }

        public DynamoDbConnection(DynamoDbConnectionStringBuilder connecitonStingBuilder)
        {
            this.ConnecitonStingBuilder = connecitonStingBuilder;
            this.ConnectionString = connecitonStingBuilder.ConnectionString;
            
            this.database = connecitonStingBuilder.InitialCatalog;
            this.datasource = connecitonStingBuilder.DataSource;
        }

        public override void ChangeDatabase(string databaseName)
        {
            this.database = databaseName;
        }

        public override void Close()
        {
            state = ConnectionState.Closed;
        }

        public override void Open()
        {
            state = ConnectionState.Open;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new DynamoDbCommand("", this);
        }
    }
}
