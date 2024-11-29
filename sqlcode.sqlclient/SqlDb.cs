using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
    public class SqlDb
    {
        private readonly string connection;

        public SqlDb(string connectionString)
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
