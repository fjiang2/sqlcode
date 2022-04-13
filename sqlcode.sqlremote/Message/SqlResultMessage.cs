﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Runtime.Serialization;

namespace Sys.Data.SqlRemote
{
    [DataContract]
    public class SqlResultMessage
    {
        [DataMember(Name = "xml", EmitDefaultValue = false)]
        public string Xml { get; set; }

        [DataMember(Name = "count", EmitDefaultValue = false)]
        public int Count { get; set; }

        [DataMember(Name = "scalar", EmitDefaultValue = false)]
        public object Scalar { get; set; }

        [DataMember(Name = "error", EmitDefaultValue = false)]
        public string Error { get; set; }

        public SqlResultMessage()
        {
        }

        public override string ToString()
        {
            return $"Count={Count}, Error={Error}";
        }
    }
}
