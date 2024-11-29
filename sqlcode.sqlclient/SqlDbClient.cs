using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
    public class SqlDbClient : IDbClient
    {
        private readonly string connection;

        public SqlDbClient(string connectionString)
        {
            connection = connectionString;
#if NET8_0
            connection += "TrustServerCertificate=True;";
#endif
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
