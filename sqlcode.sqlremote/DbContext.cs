using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Sys.Data.Entity;

namespace Sys.Data.SqlRemote
{
    public class DbContext : DataContext
    {
        public DbContext(ISqlRemoteBroker broker)
            : base(new SqlRemoteAgent(broker))
        {
        }

        public DbContext(SqlRemoteAgent agent)
            : base(agent)
        {
        }
    }
}
