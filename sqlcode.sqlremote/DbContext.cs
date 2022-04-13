using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Data.Entity;

namespace Sys.Data.SqlRedis
{
    public class DbContext : DataContext
    {
        public DbContext(string connectionString)
            : this(new SqlRedisConnectionStringBuilder(connectionString))
        {
        }

        public DbContext(SqlRedisConnectionStringBuilder connectionString)
            : base(new SqlRedisAgent(connectionString))
        {
        }

        public DbContext(SqlRedisAgent agent)
            : base(agent)
        {
        }
    }
}
