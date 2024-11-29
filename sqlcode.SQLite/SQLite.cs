﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sys.Data.Entity;
using System.IO;

namespace Sys.Data.SQLite
{
    public class SQLite
    {
        private readonly string connection;

        public SQLite(string fileName, int poolSize)
        {
            string path = Path.GetFullPath(fileName);
            connection = $"provider=sqlite;Data Source={path};Version=3; DateTimeFormat=Ticks; Pooling=True; Max Pool Size={poolSize};";
        }

        public SQLite(string connectionString)
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
