using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Sys.Data.SqlRemote
{

    public class SqlRemoteRequest
    {
        [JsonPropertyName("dbx")]
        public DbProvider Provider { get; set; }

        [JsonPropertyName("sql")]
        public string CommandText { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CommandType CommandType { get; set; }

        [JsonPropertyName("args")]
        public IList<SqlRemoteParameter> Parameters { get; set; }

        [JsonPropertyName("func")]
        public string Function { get; set; }

        [JsonPropertyName("start")]
        public int StartRecord { get; set; }

        [JsonPropertyName("maxRows")]
        public int MaxRecords { get; set; }

        public SqlRemoteRequest()
        {

        }

        public SqlRemoteRequest(DbProvider provider, string sql)
        {
            this.Provider = provider;
            this.CommandText = sql;
            this.CommandType = CommandType.Text;
            this.Parameters = new List<SqlRemoteParameter>();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"{Provider}:: {Function}(\"{CommandText}\"");

            string args = string.Join(",", Parameters.Select(x => $"@{x}"));
            if (!string.IsNullOrEmpty(args))
            {
                builder.Append($", {args}");
            }
            builder.Append($")");

            return builder.ToString();
        }
    }
}
