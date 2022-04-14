namespace Sys.Data.SqlRemote
{
    public interface ISqlMessageServer
    {
        SqlResultMessage Execute(SqlRequestMessage request);
    }
}
