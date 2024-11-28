using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Data.Entity;
#if NET8_0
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace Sys.Data.SqlClient
{
    public class DbContext : DataContext
    {
        public DbContext(string connectionString)
            : this(new SqlConnectionStringBuilder(connectionString))
        {
        }

        public DbContext(SqlConnectionStringBuilder connectionString)
            : base(new SqlDbAgent(connectionString))
        {
        }

        public DbContext(SqlDbAgent agent)
            : base(agent)
        {
        }
    }
}
