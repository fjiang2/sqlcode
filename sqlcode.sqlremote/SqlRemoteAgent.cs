using System;
using System.Data.Common;
using Sys.Data.Entity;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteAgent : DbAgent
    {
        private readonly ISqlRemoteClient client;
        
        public SqlRemoteAgent(ISqlRemoteClient client)
            : base(new DbConnectionStringBuilder())
        {
            this.client = client;
        }

        public override DbAgentOption Option => new DbAgentOption { Style = client.Style };
        public override IDbAccess Access(SqlUnit unit) => new SqlRemoteAccess(client, unit);

    }
}
