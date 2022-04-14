namespace Sys.Data.SqlRemote
{
    public interface ISqlRemoteServer
    {
        SqlRemoteResult Execute(SqlRemoteRequest request);
    }
}
