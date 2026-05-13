namespace SqlGrpcClient
{
    public class SqlApiOption
    {
        /// <summary>
        /// IP address of gRPC server
        /// </summary>
        public string Address { get; set; }

        public SqlApiOption()
        {
            Address = "https://localhost:5058";
        }
    }
}
