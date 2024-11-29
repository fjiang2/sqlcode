using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.Data.Entity;
using System.Data.Common;

#if NET8_0
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Sys.Data.SqlClient
{
    public class SqlDb
    {
        private readonly SqlConnectionStringBuilder connection;

        public SqlDb(string connectionString)
        {
            connection = new SqlConnectionStringBuilder(connectionString);
        }

        public SqlDb(SqlConnectionStringBuilder connectionString)
        {
            connection = connectionString;
        }

        public IDbAgent Agent => new SqlDbAgent(connection);
        public IDbContext Context => new DataContext(Agent);
        public DataQuery Query => new DataQuery(Agent);

        public void SetDefaultAgent()
        {
            Entity.Query.SetDefaultAgent(Agent);
        }
    }
}
