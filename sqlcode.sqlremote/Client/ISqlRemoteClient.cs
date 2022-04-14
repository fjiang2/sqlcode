using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    public interface ISqlRemoteClient
    {
        DbAgentStyle Style { get; }
        Task<SqlRemoteResult> RequesteAsync(SqlRemoteRequest request);
    }
}
