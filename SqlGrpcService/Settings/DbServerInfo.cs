using Sys.Data;

namespace SqlGrpcService.Settings
{
    public class DbServerInfo
    {
        public string Name { get; set; } = string.Empty;

        public DbAgentStyle Style { get; set; } = DbAgentStyle.SqlServer;

        public string ConnectionString { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} :: {Style} :: {ConnectionString}";
        }
    }
}

