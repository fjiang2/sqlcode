using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace sqlcode.dynamodb.extensions;

public static partial class Json
{
    public static Encoding Encoding = Encoding.UTF8;
    public static JsonNamingPolicy UpperCase = new UpperCaseNamingPolicy();

    /// <summary>
    /// Serializes a particular object to a stream.
    /// </summary>
    /// <typeparam name="T">Type of object to serialize</typeparam>
    /// <param name="obj">The object o serialize</param>
    /// <returns>Serialized string from object</returns>
    public static string Serialize<T>(T obj)
    {
        MemoryStream jsonStream = new MemoryStream();
        Serialize(obj, jsonStream);
        return Encoding.GetString(jsonStream.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="stream"></param>
    public static void Serialize<T>(T obj, Stream stream)
    {
        var serializer = new DefaultLambdaJsonSerializer(option =>
        {
            option.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            option.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            option.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        });

        serializer.Serialize(obj, stream);
    }

    /// <summary>
    ///  Deserializes a stream to a particular type.
    /// </summary>
    /// <typeparam name="T">Type of object to deserialize to.</typeparam>
    /// <param name="json">The string to deserialize</param>
    /// <returns>Deserialized object from string.</returns>
    public static T Deserialize<T>(string json)
    {
        byte[] bytes = Encoding.GetBytes(json);
        MemoryStream jsonStream = new MemoryStream(bytes);

        return Deserialize<T>(jsonStream);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static T Deserialize<T>(Stream stream)
    {
        var serializer = new DefaultLambdaJsonSerializer(option =>
        {
            option.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            option.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            option.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        });

        var obj = serializer.Deserialize<T>(stream);
        return obj;
    }

    public static string SerializeAndPrettify(object value)
    {
        var option = new JsonSerializerOptions
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            //ReadCommentHandling = JsonCommentHandling.Allow,
        };

        option.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        return JsonSerializer.Serialize(value, option);
    }
}
