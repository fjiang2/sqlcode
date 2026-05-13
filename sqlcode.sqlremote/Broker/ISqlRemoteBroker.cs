using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    public interface ISqlRemoteBroker
    {
        string ServerName { get; }
        DbAgentStyle Style { get; }
        Task<SqlRemoteResult> RequestAsync(SqlRemoteRequest request);
    }
}
