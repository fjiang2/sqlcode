using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Sys.Data.SqlRemote
{
    public static class Json
    {
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static bool TryDeserialize<T>(string json, out T result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }

        public static T Deserialize<T>(string json, JsonConverter converter)
        {
            return JsonConvert.DeserializeObject<T>(json, converter);
        }

        public static T Deserialize<T>(string json, T definition)
        {
            return JsonConvert.DeserializeAnonymousType(json, definition);
        }


        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static string Serialize(object obj, JsonConverter converter)
        {
            return JsonConvert.SerializeObject(obj, converter);
        }

        public static void MergeTo(string json, object target)
        {
            JsonConvert.PopulateObject(json, target);
        }

        public static string SerializeAndFormat(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

       
    }
}

