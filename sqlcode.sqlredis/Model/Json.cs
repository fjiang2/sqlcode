using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NET48
using System.IO;
using System.Runtime.Serialization.Json;

namespace Sys.Data.SqlRedis
{
    public static class Json
    {
        public static T Deserialize<T>(this string json)
        {
            return (T)Deserialize(typeof(T), json);
        }

        public static string Serialize<T>(this T graph)
        {
            return Serialize(typeof(T), graph);
        }

        public static object Deserialize(Type type, string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(json);
                writer.Flush();
                stream.Position = 0;
                return serializer.ReadObject(stream);
            }
        }

        public static string Serialize(Type type, object graph)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, graph);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

    }
}


#else
using System.Text.Json;


namespace Sys.Data.SqlRedis
{
    internal static class Json
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
#endif
