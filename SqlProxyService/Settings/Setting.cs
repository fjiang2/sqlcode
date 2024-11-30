using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlProxy.Service.Settings
{
    public class Setting : ISetting
    {
        public ServerOption ServerOption { get; }

        public Setting(IConfiguration configuration)
        {
            var proxy = configuration.GetSection("Proxy");
            this.ServerOption = GetProxy(proxy);
        }

        private static ServerOption GetProxy(IConfigurationSection? section)
        {
            var prefixes = section?
                .GetSection("Url")
                .GetChildren()
                .Select(c => c.Value ?? string.Empty)
                .ToArray();

            if (prefixes == null)
            {
                Console.Error.WriteLine($"Undefined \"Url\" in appsettings");
            }

            var serverOption = new ServerOption
            {
                Prefixes = prefixes ?? new string[] { "http://localhost/sqlhandler/" },
                DbServers = new List<DbServerInfo>(),
            };

            GetDbServers(section, serverOption.DbServers);
            return serverOption;
        }

        private static void GetDbServers(IConfigurationSection? section, List<DbServerInfo> dbServers)
        {
            var servers = section?.GetSection("Servers").GetChildren();

            if (servers == null)
            {
                Console.Error.WriteLine($"Undefined \"Servers\" in appsettings");
                return;
            }

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
                Console.Error.WriteLine($"Invalid provider: {provider}");
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

