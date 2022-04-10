using System.Data;
using System.Data.Common;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteDataAdapter : DbDataAdapter
    {
        SqlRemoteCommand command;

        public SqlRemoteDataAdapter(SqlRemoteCommand command)
        {
            this.command = command; 
        }

        public override int Fill(DataSet dataSet)
        {
            var connection = (SqlRemoteConnection)command.Connection;
            
            string sql = command.CommandText;
            var args = command.Parameters;

            return 0;
        }
    }
}
