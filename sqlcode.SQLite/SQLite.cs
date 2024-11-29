using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using System.Data.SQLite;
using Sys.Data.Entity;
using System.Drawing;
using System.IO;

namespace Sys.Data.SQLite
{
    public class SQLite
    {
        private readonly SQLiteConnectionStringBuilder connection;

        public SQLite(string fileName, int poolSize)
        {
            string path = Path.GetFullPath(fileName);
            connection = new SQLiteConnectionStringBuilder($"provider=sqlite;Data Source={path};Version=3; DateTimeFormat=Ticks; Pooling=True; Max Pool Size={poolSize};");
        }

        public SQLite(string connectionString)
        {
            connection = new SQLiteConnectionStringBuilder(connectionString);
        }

        public SQLite(SQLiteConnectionStringBuilder connectionString)
        {
            connection = connectionString;
        }

        public IDbAgent Agent => new SQLiteAgent(connection);
        public IDbContext Context => new DataContext(Agent);
        public DataQuery Query => new DataQuery(Agent);

        public void SetDefaultAgent()
        {
            Entity.Query.SetDefaultAgent(Agent);
        }
    }
}
