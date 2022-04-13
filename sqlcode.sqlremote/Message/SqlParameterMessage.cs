﻿using System.Data;
using System.Runtime.Serialization;

namespace Sys.Data.SqlRemote
{
    [DataContract]
    public class SqlParameterMessage : IDataParameter
    {
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string ParameterName { get; set; }

        [DataMember(Name = "value", EmitDefaultValue = false)]
        public object Value { get; set; }

        [DataMember(Name = "direction", EmitDefaultValue = false)]
        public ParameterDirection Direction { get; set; }


        public DbType DbType { get; set;}
        public bool IsNullable { get; set; }
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }

        public override string ToString()
        {
            if (Direction == ParameterDirection.Input)
                return $"{ParameterName} = in {Value}";
            else if (Direction == ParameterDirection.Output)
                return $"{ParameterName} = out {Value}";
            else if (Direction == ParameterDirection.InputOutput)
                return $"{ParameterName} = in-out {Value}";
            else
                return $"{ParameterName} = return {Value}";
        }
    }
}
