using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    public interface ISqlRemoteBroker
    {
        string ProviderName { get; }
        DbAgentStyle Style { get; }
        Task<SqlRemoteResult> RequestAsync(SqlRemoteRequest request);
    }
}
