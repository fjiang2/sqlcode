using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.Data;

namespace SqlProxyService.Settings
{
    public class Setting : ISetting
    {
        public ServerOption ServerOption { get; }

        public Setting(IConfiguration configuration)
        {
            var section = configuration.GetSection("Proxy");
            var dbServers = new List<DbServerInfo>();

            ServerOption = new ServerOption
            {
                Prefixes = section?
                    .GetSection("Url")
                    .GetChildren()
                    .Select(c => c.Value ?? string.Empty)
                    .ToArray() ?? new string[] { "http://localhost/sqlhandler/" },

                DbServers = dbServers,
            };

            GetDbServers(section, dbServers);
        }

        private static void GetDbServers(IConfigurationSection? section, List<DbServerInfo> dbServers)
        {
            var servers = section?.GetSection("Servers").GetChildren();

            if (servers == null)
                return;

            foreach (var server in servers)
            {
                DbServerInfo? dbServerInfo = GetDbServerInfo(server);
                string name = server?.GetValue<string>("Name") ?? string.Empty;

                if (dbServerInfo != null)
                {
                    dbServers.Add(dbServerInfo);
                }
            }
        }

        private static DbServerInfo? GetDbServerInfo(IConfigurationSection? server)
        {
            string name = server?.GetValue<string>("Name") ?? string.Empty;
            string? provider = server?.GetValue<string>("Provider");
            if (!Enum.TryParse<DbAgentStyle>(provider, ignoreCase: true, out var style))
            {
                Console.Error.WriteLine($"Invalid Provider :{provider}");
                return null;
            }

            DbServerInfo dbServerInfo = new DbServerInfo
            {
                Name = name,
                Style = style,
                ConnectionString = server?.GetValue<string>("ConnectionString") ?? string.Empty,
            };

            if (style == DbAgentStyle.SqlServer)
                dbServerInfo.ConnectionString += "TrustServerCertificate=True;";

            return dbServerInfo;
        }
    }
}

