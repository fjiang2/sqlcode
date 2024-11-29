using System;
using System.Data.Common;
using Sys.Data.Entity;

namespace Sys.Data.SqlRemote
{
    internal class SqlRemoteAgent : IDbAgent
    {
        private readonly ISqlRemoteBroker broker;
        
        public SqlRemoteAgent(ISqlRemoteBroker broker)
        {
            this.broker = broker;
        }

        public DbAgentOption Option => new DbAgentOption { Style = broker.Style };
        public IDbAccess Access(SqlUnit unit) => new SqlRemoteAccess(broker, unit);
        
        public override string ToString()
        {
            return $"{broker}";
        }
    }
}
