using Sys.Data;

namespace SqlProxyService.Settings
{
    public class ServerOption
    {
        public string[] Prefixes { get; set; } = new string[0];

        public string ConnectionString { get; set; } = string.Empty;
    }
}

