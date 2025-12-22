using System.Data;
using System.Text.Json.Serialization;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteParameter : IDataParameter
    {
        [JsonPropertyName("name")]
        public string ParameterName { get; set; }

        [JsonPropertyName("value")]
        public object Value { get; set; }

        [JsonPropertyName("direction")]
        public ParameterDirection Direction { get; set; }

        [JsonPropertyName("type")]
        public DbType DbType { get; set;}

        [JsonPropertyName("nullable")]
        public bool IsNullable { get; set; }
        
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }

        public override string ToString()
        {
            if (Direction == ParameterDirection.Input)
                return $"{ParameterName}={Value}";
            else if (Direction == ParameterDirection.Output)
                return $"{ParameterName}=out {Value}";
            else if (Direction == ParameterDirection.InputOutput)
                return $"{ParameterName}=ref {Value}";
            else
                return $"{ParameterName}=ret {Value}";
        }
    }
}
