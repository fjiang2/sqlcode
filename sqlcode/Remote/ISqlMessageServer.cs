namespace Sys.Data.SqlRemote
{
    public interface ISqlMessageServer
    {
        void Receive(SqlRequestMessage request);
    }
}
