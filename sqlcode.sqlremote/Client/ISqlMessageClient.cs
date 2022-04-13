using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    public interface ISqlMessageClient
    {
        Task<SqlResultMessage> RequesteAsync(SqlRequestMessage request);
    }
}
