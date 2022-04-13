﻿using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Sys.Data.SqlRemote
{
    [DataContract]
    public class SqlRequest
    {
        [DataMember(Name = "sql", EmitDefaultValue = false)]
        public string CommandText { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false)]
        public CommandType CommandType { get; set; }

        [DataMember(Name = "args", EmitDefaultValue = false)]
        public List<SqlArgument> Parameters { get; } 
        
        [DataMember(Name = "func", EmitDefaultValue = false)]
        public string Function { get; set; }

        [DataMember(Name = "startrow", EmitDefaultValue = false)]
        public int StartRecord { get; set; }

        [DataMember(Name = "maxrow", EmitDefaultValue = false)]
        public int MaxRecords { get; set; }

        public SqlRequest(string sql)
        {
            this.CommandText = sql;
            this.CommandType = CommandType.Text;
            this.Parameters = new List<SqlArgument>();
        }
    }
}
