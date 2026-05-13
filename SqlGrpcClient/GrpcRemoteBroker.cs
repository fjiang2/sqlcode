using Grpc.Core;
using Grpc.Net.Client;
using Sys.Data.SqlRemote;

namespace SqlGrpcClient
{
    public class GrpcRemoteBroker : SqlRemoteBroker
    {
        private static readonly string? token;

        private readonly SqlApi.SqlApiClient client;

        public GrpcRemoteBroker(string address)
        {
            //var handler = new HttpClientHandler();
            //handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            //var x = new GrpcChannelOptions { HttpHandler = handler };

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var channel = CreateAuthenticatedChannel(address);

            this.client = new SqlApi.SqlApiClient(channel);
        }


        private static GrpcChannel CreateAuthenticatedChannel(string address)
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(token))
                {
                    metadata.Add("Authorization", $"Bearer {token}");
                }
                return Task.CompletedTask;
            });

            var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });
            return channel;
        }

        public override async Task<SqlRemoteResult> RequestAsync(SqlRemoteRequest request)
        {
            SqlRequest _request = new SqlRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                Body = Json.Serialize(request)
            };
            SqlResponse response = await client.ExecuteAsync(_request);

            SqlRemoteResult result = Json.Deserialize<SqlRemoteResult>(response.Result);
            return result;
        }


        public static SqlRemoteClient CreateRemoteClient(GrpcApiOption option)
        {
            var dbClient = new SqlRemoteClient(new GrpcRemoteBroker(option.Address)
            {
                ServerName = option.ServerName,
                Style = option.Style,
            });

            return dbClient;
        }
    }
}
