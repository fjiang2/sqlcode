using Sys.Data.SqlRemote;

namespace SqlGrpcClient
{
    public class GrpcRemoteBroker : SqlRemoteBroker
    {
        public override Task<SqlRemoteResult> RequestAsync(SqlRemoteRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
