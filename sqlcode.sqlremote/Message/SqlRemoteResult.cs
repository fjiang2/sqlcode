using System;
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
    public class SqlRemoteResult
    {
        /// <summary>
        /// Result of ExecuteNonQuery()
        /// </summary>
        [DataMember(Name = "count", EmitDefaultValue = false)]
        public int Count { get; set; }


        /// <summary>
        /// Result of ExecuteScalar()
        /// </summary>
        [DataMember(Name = "scalar", EmitDefaultValue = false)]
        public object Scalar { get; set; }

        /// <summary>
        /// Serialized text from DataTable or DataSet
        /// </summary>
        [DataMember(Name = "data", EmitDefaultValue = false)]
        public string Xml { get; set; }


        /// <summary>
        /// Exception
        /// </summary>
        [DataMember(Name = "error", EmitDefaultValue = false)]
        public string Error { get; set; }

        public SqlRemoteResult()
        {
        }

        public override string ToString()
        {
            return $"Count={Count}, Scalar={Scalar}, Data-Length={Xml?.Length}, Error={Error}";
        }
    }
}
