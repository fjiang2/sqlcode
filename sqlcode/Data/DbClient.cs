using System;
using System.Collections.Generic;
using System.Text;
using Sys.Data.Entity;

namespace Sys.Data
{
    public class DbClient : IDbClient
    {
        public DbClient(IDbAgent agent)
        {
            Agent = agent;
        }

        public IDbAgent Agent { get; }
        public IDbContext Context => new DbContext(Agent);
        public IDbQuery Query => new DbQuery(Agent);

    }
}
