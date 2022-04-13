using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Sys.Data.SqlRemote
{
    [DataContract]
    public class SqlRequestMessage
    {
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public Guid RequestId { get; set; }

        [DataMember(Name = "sql", EmitDefaultValue = false)]
        public string CommandText { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false)]
        public CommandType CommandType { get; set; }

        [DataMember(Name = "args", EmitDefaultValue = false)]
        public List<SqlParameterMessage> Parameters { get; } 
        
        [DataMember(Name = "func", EmitDefaultValue = false)]
        public string Function { get; set; }

        [DataMember(Name = "startrow", EmitDefaultValue = false)]
        public int StartRecord { get; set; }

        [DataMember(Name = "maxrow", EmitDefaultValue = false)]
        public int MaxRecords { get; set; }

        public SqlRequestMessage(string sql)
        {
            this.RequestId = Guid.NewGuid();
            this.CommandText = sql;
            this.CommandType = CommandType.Text;
            this.Parameters = new List<SqlParameterMessage>();
        }
    }
}
