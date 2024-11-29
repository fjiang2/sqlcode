using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Sys.Data.SqlRemote
{

    [DataContract]
    public class SqlRemoteRequest
    {
        [DataMember(Name = "dbx", EmitDefaultValue = false)]
        public DbProvider Provider { get; set; }

        [DataMember(Name = "sql", EmitDefaultValue = false)]
        public string CommandText { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false)]
        public CommandType CommandType { get; set; }

        [DataMember(Name = "args", EmitDefaultValue = false)]
        public IList<SqlRemoteParameter> Parameters { get; set; }

        [DataMember(Name = "func", EmitDefaultValue = false)]
        public string Function { get; set; }

        [DataMember(Name = "start", EmitDefaultValue = false)]
        public int StartRecord { get; set; }

        [DataMember(Name = "maxRows", EmitDefaultValue = false)]
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
            if(!string.IsNullOrEmpty(args))
                builder.Append($", {args}");
            builder.Append($")");

            return builder.ToString();
        }
    }
}
