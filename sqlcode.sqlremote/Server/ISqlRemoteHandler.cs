namespace Sys.Data.SqlRemote
{
    public interface ISqlRemoteHandler
    {
        SqlRemoteResult Execute(SqlRemoteRequest request);
    }
}
