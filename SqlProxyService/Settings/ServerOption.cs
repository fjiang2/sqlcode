namespace SqlProxyService.Settings
{
    public class ServerOption
    {
        public string[] Prefixes { get; set; } = new string[0];

        public List<DbServerInfo> DbServers { get; set; } = new();
    }
}

