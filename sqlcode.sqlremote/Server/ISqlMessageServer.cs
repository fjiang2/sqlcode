namespace Sys.Data.SqlRemote
{
    public interface ISqlMessageServer
    {
        void OnRequest(SqlRequestMessage request);
    }
}
