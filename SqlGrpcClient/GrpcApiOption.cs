using Sys.Data;

namespace SqlGrpcClient
{
    public class GrpcApiOption
    {
        /// <summary>
        /// IP address of gRPC server
        /// </summary>
        public string Address { get; set; }

        public string ProviderName { get; set; } = string.Empty;

        /// <summary>
        /// Database Server style, default is SqlServer
        /// </summary>
        public DbAgentStyle Style { get; set; } = DbAgentStyle.SqlServer;

        public GrpcApiOption()
        {
            Address = "https://localhost:5058";
        }

        public override string ToString()
        {
            return Address;
        }
    }
}
