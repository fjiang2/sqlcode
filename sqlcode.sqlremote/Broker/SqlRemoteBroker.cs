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

        public string ServerName { get; set; } = string.Empty;

        public DbAgentStyle Style { get; set; } = DbAgentOption.DefaultStyle;

        public abstract Task<SqlRemoteResult> RequestAsync(SqlRemoteRequest request);


        public override string ToString()
        {
            return $"ServerName: {ServerName}, Style: {Style}";
        }
    }
}
