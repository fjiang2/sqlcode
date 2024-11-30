using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sys.Data.Entity;
using System.IO;

namespace Sys.Data.SQLite
{
    public class SQLiteClient : IDbClient
    {
        private readonly string connection;

        public SQLiteClient(string fileName, int poolSize)
        {
            string path = Path.GetFullPath(fileName);
            connection = $"provider=sqlite;Data Source={path};Version=3; DateTimeFormat=Ticks; Pooling=True; Max Pool Size={poolSize};";
        }

        public SQLiteClient(string connectionString)
        {
            connection = connectionString;
        }

        public IDbAgent Agent => new SQLiteAgent(connection);
        public IDbContext Context => new DbContext(Agent);
        public DbQuery Query => new DbQuery(Agent);

        public void SetDefaultAgent()
        {
            Entity.Query.SetDefaultAgent(Agent);
        }
    }
}
