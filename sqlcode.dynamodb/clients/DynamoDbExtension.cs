using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace sqlcode.dynamodb.clients
{
    internal static class DynamoDbExtension
    {
        const string _DEFAULT_KEY = "Value";

        public static Dictionary<string, AttributeValue> ToAttributes(this JsonNode node)
        {
            if (node is JsonObject)
            {
                string json = node.ToString();
                var doc = Document.FromJson(json);
                var attribues = doc.ToAttributeMap();
                return attribues;
            }
            else
            {
                JsonObject jsonObject = new JsonObject();
                jsonObject.Add(_DEFAULT_KEY, node.Clone());
                return ToAttributes(jsonObject);
            }
        }

        public static JsonNode? ToJsonNode(this Dictionary<string, AttributeValue> attributes)
        {
            var doc = Document.FromAttributeMap(attributes);
            string json = doc.ToJson();
            return JsonNode.Parse(json);
        }

        private static JsonNode Clone(this JsonNode node)
        {
            if (node.Parent == null)
            {
                return node;
            }
            else
            {
                return JsonNode.Parse(node.ToJsonString())!;
            }
        }


        public static AttributeValue CreateAttribute(this object obj)
        {
            string json = JsonSerializer.Serialize(new { attribute = obj });
            var doc = Document.FromJson(json);
            Dictionary<string, AttributeValue> attributes = doc.ToAttributeMap();
            return attributes["attribute"];
        }

        public static object? ToHostObject(this JsonValue jsonValue, Type type)
        {
            var nullable = Nullable.GetUnderlyingType(type);
            if (nullable != null)
            {
                type = nullable;
            }
            else { }

            if (type.IsSubclassOf(typeof(Enum)))
            {
                return Enum.Parse(type, jsonValue.ToString(), ignoreCase: true);
            }
            else if (jsonValue.TryGetValue<DateTime>(out var dateTimeValue))
            {
                return dateTimeValue;
            }
            else if (jsonValue.TryGetValue<bool>(out var boolValue))
            {
                return boolValue;
            }
            else if (jsonValue.TryGetValue<string>(out var stringValue))
            {
                return stringValue;
            }
            else if (jsonValue.TryGetValue<long>(out var longValue))
            {
                return Convert.ChangeType(longValue, type);
            }
            else
            {
                throw new NotSupportedException($"Cannot get host object from {jsonValue}");
            }
        }
    }
}
