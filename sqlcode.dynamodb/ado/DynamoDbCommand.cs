using System.Data;
using System.Data.Common;

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

        public DynamoDbCommand()
        {
            CommandText = string.Empty;
        }

        public DynamoDbCommand(string commandText)
        {
            CommandText = commandText;
        }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public override object? ExecuteScalar()
        {
            throw new NotImplementedException();
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
