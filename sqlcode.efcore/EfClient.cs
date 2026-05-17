using System;
using System.Collections.Generic;
using System.Text;
using Sys.Data.Entity;

namespace Sys.Data.EfCore
{
    public class EfClient : IDbClient
    {
        private readonly IDbClient client;
        public EfClient(IDbClient client)
        {
            this.client = client;
        }

        public IDbAgent Agent => client.Agent;
        public IDbContext Context => new EfContext(Agent);
        public IDbQuery Query => client.Query;

    }
}
