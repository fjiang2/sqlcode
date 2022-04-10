using System.Data;
using System.Data.Common;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteConnection : DbConnection
    {
        private string database;
        private ConnectionState state = ConnectionState.Closed;

        public SqlRemoteConnection(string connectionString)
        {

        }

        public override string ConnectionString { get; set; }

        public override string Database { get; }

        public override string DataSource { get; }

        public override string ServerVersion { get; }

        public override ConnectionState State => state;

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
            return null;
        }

        protected override DbCommand CreateDbCommand()
        {
            return new SqlRemoteCommand("");
        }
    }
}
