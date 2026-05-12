using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Sys.Data.SqlRemote
{
    public class SqlRemoteResult
    {
        [JsonPropertyName("id")]
        public string RequestId { get; set; }

        /// <summary>
        /// Result of ExecuteNonQuery()
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }


        /// <summary>
        /// Result of ExecuteScalar()
        /// </summary>
        [JsonPropertyName("scalar")]
        public object Scalar { get; set; }

        /// <summary>
        /// Serialized text from DataTable or DataSet
        /// </summary>
        [JsonPropertyName("data")]
        public string Result { get; set; }


        /// <summary>
        /// Exception
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }

        public SqlRemoteResult()
        {
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (Scalar == null)
                builder.Append($"Row-Count={Count}");
            else
                builder.Append($"Scalar={Scalar}");

            if (Result != null)
                builder.Append($", Data-Length={Result?.Length}");

            if (!string.IsNullOrWhiteSpace(Error))
                builder.Append($", Error=\"{Error}\"");

            return builder.ToString();
        }
    }
}
