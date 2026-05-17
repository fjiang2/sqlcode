using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Sys.Data.Entity;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteClient : IDbClient
    {
        private readonly ISqlRemoteBroker connection;

        public SqlRemoteClient(string url, DbAgentStyle style, string providerName)
        {
            this.connection = new SqlHttpBroker(url)
            {
                ServerName = providerName,
                Style = style,
            };
        }

        public SqlRemoteClient(ISqlRemoteBroker remoteBroker)
        {
            this.connection = remoteBroker;
        }

        public IDbAgent Agent => new SqlRemoteAgent(connection);
        public IDbContext Context => new DbContext(Agent);
        public IDbQuery Query => new DbQuery(Agent);
    }
}
