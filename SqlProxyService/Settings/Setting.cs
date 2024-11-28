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
            var section = configuration.GetSection("Server");
            string? engine = section?.GetValue<string>("Engine");

            if (!Enum.TryParse(engine, ignoreCase: true, out DbAgentStyle style))
            {
                style = DbAgentStyle.SqlServer;
            }

            ServerOption = new ServerOption
            {
                Style = style,
                Prefixes = section?.GetSection("Url")
                .GetChildren()
                .Select(c => c.Value ?? string.Empty)
                .ToArray() ?? new string[] { "http://localhost/sqlhandler/" },

                ConnectionString = section?.GetValue<string>("ConnectionString") ?? "Server = (LocalDB)\\MSSQLLocalDB;initial catalog=Northwind;Integrated Security=true;",
            };
        }
    }
}

