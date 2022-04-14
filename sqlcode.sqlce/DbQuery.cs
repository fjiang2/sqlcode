
using System.Data.SqlServerCe;
using Sys.Data.Entity;

namespace Sys.Data.SqlCe
{
    public class DbQuery : DataQuery
    {
        public DbQuery(string connectionString)
            : this(new SqlCeConnectionStringBuilder(connectionString))
        {
        }


        public DbQuery(SqlCeConnectionStringBuilder connectionString)
            : base(new SqlCeAgent(connectionString))
        {
        }

        public DbQuery(SqlCeAgent agent)
           : base(agent)
        {
        }


    }
}
