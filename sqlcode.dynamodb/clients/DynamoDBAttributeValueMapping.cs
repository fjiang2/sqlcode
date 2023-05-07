using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal.Transform;
using sqlcode.dynamodb.entities;

namespace sqlcode.dynamodb.clients
{
    static class DynamoDBAttributeValueMapping
    {
        private static EntityValue MapNumber(string value)
        {
            if (int.TryParse(value, out var x))
            {
                return new EntityValue(x);
            }
            else if (long.TryParse(value, out var x1))
            {
                return new EntityValue(x1);
            }
            else
            {
                return new EntityValue(double.Parse(value));
            }
        }

        private static EntityValue MapValue(AttributeValue value)
        {
            if (value.NULL)
                return new EntityValue(DBNull.Value);

            // string
            if (value.S != null)
                return new EntityValue(value.S);

            // boolean
            if (value.IsBOOLSet)
                return new EntityValue(value.BOOL);

            // number
            if (value.N != null)
            {
                return MapNumber(value.N);
            }

            // List<object>
            if (value.IsLSet)
                return new EntityValue(typeof(List<object>), MapList(value.L));

            // Dictionary<string,object>
            if (value.IsMSet)
                return new EntityValue(typeof(Dictionary<string, object>), MapDictionary(value.M));

            // List<int>, List<double>, ...
            if (value.NS != null && value.NS.Count > 0)
            {
                var L = value.NS.Select(x => MapNumber(x)).ToList();
                Type type = typeof(List<double>);

                if (L.All(x => x.Type == typeof(int)))
                    type = typeof(List<int>);
                else if (L.All(x => x.Type == typeof(long)))
                    type = typeof(List<long>);

                return new EntityValue(type, L);
            }

            // List<string>
            if (value.SS != null && value.SS.Count > 0)
            {
                return new EntityValue(typeof(List<string>), value.SS);
            }

            throw new Exception($"Cannot process attribute value: {value}");
        }

        private static List<object> MapList(IEnumerable<AttributeValue> attributeList)
        {
            List<object> list = new List<object>();
            foreach (AttributeValue value in attributeList)
            {
                list.Add(MapValue(value).Value);
            }

            return list;
        }

        private static Dictionary<string, object> MapDictionary(Dictionary<string, AttributeValue> item)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (KeyValuePair<string, AttributeValue> kvp in item)
            {
                string name = kvp.Key;
                AttributeValue value = kvp.Value;
                dict.Add(name, MapValue(value).Value);
            }

            return dict;
        }

        private static EntityRow MapRow(Dictionary<string, AttributeValue> item)
        {
            EntityRow row = new EntityRow();
            foreach (KeyValuePair<string, AttributeValue> kvp in item)
            {
                string name = kvp.Key;
                var value = MapValue(kvp.Value);
                row.Add(name, value);
            }

            return row;
        }

        public static EntityTable MapTable(this EntityTable table, IEnumerable<Dictionary<string, AttributeValue>> items)
        {
            foreach (Dictionary<string, AttributeValue> item in items)
            {
                var row = MapRow(item);
                table.Add(row);
            }
            return table;
        }

        public static EntityTable MapTable(this IEnumerable<Dictionary<string, AttributeValue>> items)
        {
            EntityTable rows = new EntityTable();
            return rows.MapTable(items);
        }
    }
}
