using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    public interface ISqlRemoteBroker
    {
        DbAgentStyle Style { get; }
        Task<SqlRemoteResult> RequestAsync(SqlRemoteRequest request);
    }
}
