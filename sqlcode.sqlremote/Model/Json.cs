using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sys.Data.SqlRemote
{
    public static class Json
    {
        public static T Deserialize<T>(string json)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static T Deserialize<T>(string json, T definition)
        {
            return JsonConvert.DeserializeAnonymousType(json, definition);
        }


        public static string Serialize(object obj, bool indented = false)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = indented ? Formatting.Indented : Formatting.None,
            };

            settings.Converters.Add(new StringEnumConverter());
            return JsonConvert.SerializeObject(obj, settings);
        }

    }
}

