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
    public class SqlCe
    {
        private readonly SqlCeConnectionStringBuilder connection;

        public SqlCe(string fileName, int bufferSize)
        {
            string path = Path.GetFullPath(fileName);
            connection = new SqlCeConnectionStringBuilder($"Data Source={path};Max Buffer Size={bufferSize};Persist Security Info=False;");
        }

        public SqlCe(string connectionString)
        {
            connection = new SqlCeConnectionStringBuilder(connectionString);
        }

        public SqlCe(SqlCeConnectionStringBuilder connectionString)
        {
            connection = connectionString;
        }

        public IDbAgent Agent => new SqlCeAgent(connection);
        public IDbContext Context => new DataContext(Agent);
        public DataQuery Query => new DataQuery(Agent);

        public void SetDefaultAgent()
        {
            Entity.Query.SetDefaultAgent(Agent);
        }
    }
}
