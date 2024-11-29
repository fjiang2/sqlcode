using System;
using System.Data.Common;
using Sys.Data.Entity;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteAgent : DbAgent
    {
        private readonly ISqlRemoteBroker broker;
        
        public SqlRemoteAgent(ISqlRemoteBroker broker)
            : base("N/A")
        {
            this.broker = broker;
        }

        public override DbAgentOption Option => new DbAgentOption { Style = broker.Style };
        public override IDbAccess Access(SqlUnit unit) => new SqlRemoteAccess(broker, unit);

    }
}
