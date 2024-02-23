using System.Data;
using System.Data.Common;
using mudu.aws.core.clients;
using mudu.aws.core;

namespace sqlcode.dynamodb.ado
{

    class DynamoDbCommand : DbCommand
    {
        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; } = CommandType.Text;
        public override bool DesignTimeVisible { get; set; } = false;
        public override UpdateRowSource UpdatedRowSource { get; set; } = UpdateRowSource.None;
        protected override DbConnection? DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection { get => throw new Exception(); }

        protected override DbTransaction? DbTransaction { get; set; }

        private readonly IDbClient dbClient;
        private readonly PartiViewQuery query;

        public DynamoDbCommand(string cmdText, DynamoDbConnection connection)
        {
            CommandText = cmdText;
            this.CommandType = CommandType.Text;
            this.DbConnection = connection;

            var connectionString = connection.ConnectionStringBuilder;
            IAccount account = connectionString.Account;
            this.dbClient = new DbClient(account);
            this.query = new PartiViewQuery(dbClient, connectionString.InitialCatalog);
        }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            dbClient.ExecuteNonQueryAsync(CommandText).Wait();
            return 1;
        }

        public override object? ExecuteScalar()
        {
            var dt = query.FillDataTableAsync(CommandText, editable: true, maxRows: 10).Result;
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                if (row.ItemArray != null && row.ItemArray.Length > 0)
                    return row.ItemArray[0];
            }

            return null;
        }

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }
    }
}
