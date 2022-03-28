using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
    public class DbContext : DataContext
    {
        public DbContext(string connectionString)
            : this(new SqlConnectionStringBuilder(connectionString))
        {
        }

        public DbContext(SqlConnectionStringBuilder connectionString)
            : this(new SqlDbAgent(connectionString))
        {
        }

        public DbContext(SqlDbAgent agent)
            : base(agent)
        {
        }
    }
}
