using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    public abstract class SqlRemoteBroker : ISqlRemoteBroker
    {
        public SqlRemoteBroker() 
        { 
        }

        public string ProviderName { get; set; } = string.Empty;

        public DbAgentStyle Style { get; set; } = DbAgentOption.DefaultStyle;

        public abstract Task<SqlRemoteResult> RequestAsync(SqlRemoteRequest request);
    
    }
}
