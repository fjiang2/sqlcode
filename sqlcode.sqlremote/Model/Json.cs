using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;


namespace Sys.Data.SqlRemote
{
    public static class Json
    {
        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static bool TryDeserialize<T>(string json, out T result)
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(json);
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }

        public static T Deserialize<T>(string json, JsonSerializerOptions converter)
        {
            return JsonSerializer.Deserialize<T>(json, converter);
        }

        public static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static string Serialize(object obj, bool indented)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        }

        public static string Serialize(object obj, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize(obj, options);
        }

        public static string SerializeAndFormat(object obj)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(obj, options);
        }

    }
}

