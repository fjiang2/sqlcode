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

            ServerOption = new ServerOption
            {
                Prefixes = section?.GetSection("Url")
                .GetChildren()
                .Select(c => c.Value ?? string.Empty)
                .ToArray() ?? new string[] { "http://localhost/sqlhandler/" },

                ConnectionString = section?.GetValue<string>("ConnectionString") ?? "Server = (LocalDB)\\MSSQLLocalDB;initial catalog=Northwind;Integrated Security=true;",
            };
        }
    }
}

