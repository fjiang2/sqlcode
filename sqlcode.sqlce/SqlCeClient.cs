using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.Data.Entity;

namespace Sys.Data.SqlCe
{
    public class SqlCeClient : IDbClient
    {
        private readonly string connection;

        public SqlCeClient(string fileName, int bufferSize)
        {
            string path = Path.GetFullPath(fileName);
            connection = $"Data Source={path};Max Buffer Size={bufferSize};Persist Security Info=False;";
        }

        public SqlCeClient(string connectionString)
        {
            connection = connectionString;
        }

        public IDbAgent Agent => new SqlCeAgent(connection);
        public IDbContext Context => new DbContext(Agent);
        public DbQuery Query => new DbQuery(Agent);

        public void SetDefaultAgent()
        {
            Entity.Query.SetDefaultAgent(Agent);
        }
    }
}
