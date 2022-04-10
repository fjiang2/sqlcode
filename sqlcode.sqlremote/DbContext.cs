using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Data.Entity;

namespace Sys.Data.SqlRemote
{
    public class DbContext : DataContext
    {
        public DbContext(string connectionString)
            : this(new SqlRemoteConnectionStringBuilder(connectionString))
        {
        }

        public DbContext(SqlRemoteConnectionStringBuilder connectionString)
            : base(new SqlRemoteAgent(connectionString))
        {
        }

        public DbContext(SqlRemoteAgent agent)
            : base(agent)
        {
        }
    }
}
