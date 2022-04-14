using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    public interface ISqlRemoteClient
    {
        Task<SqlRemoteResult> RequesteAsync(SqlRemoteRequest request);
    }
}
