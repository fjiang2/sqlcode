using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteCommand : DbCommand
    {
        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection { get; }
        protected override DbTransaction DbTransaction { get; set; }

        public SqlRemoteCommand(string commandText)
        {
            this.CommandText = commandText;
            this.CommandType = CommandType.Text;
        }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            return -1;
        }

        public override object ExecuteScalar()
        {
            return null;
        }

        public override void Prepare()
        {

        }
        protected override DbParameter CreateDbParameter()
        {
            return null;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return null;
        }
    }
}
