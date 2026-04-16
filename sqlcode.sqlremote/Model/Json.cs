using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sys.Data.SqlRemote
{
    public static class Json
    {
        private static JsonSerializerOptions Options
        {
            get
            {
                var option = new JsonSerializerOptions
                {
                    WriteIndented = false,
                    AllowTrailingCommas = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    //IgnoreReadOnlyProperties = true,
                    //ReadCommentHandling = JsonCommentHandling.Allow,

                };

                option.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                return option;
            }
        }

        public static string Serialize(object value, bool indented = false)
        {
            var options = Options;
            options.WriteIndented = indented;
            return JsonSerializer.Serialize(value, options);
        }


        public static T Deserialize<T>(string json)
        {
            var obj = JsonSerializer.Deserialize<T>(json, Options);
            return obj;
        }


    }
}

