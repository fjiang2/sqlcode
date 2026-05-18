using System;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data.Entity
{
    public class PocoSchema
    {
        private static readonly IDictionary<Type, ITableSchema> schemas = new Dictionary<Type, ITableSchema>();

        public static ITableSchema GetSchema(Type type)
        {
            if (schemas.TryGetValue(type, out ITableSchema schema))
                return schema;

            return null;
        }

        public static void Register(Type type, ITableSchema schema)
        {
            if (schemas.ContainsKey(type))
                schemas[type] = schema;
            else
                schemas.Add(type, schema);
        }

        public static void Unregister(Type type)
        {
            if (schemas.ContainsKey(type))
                schemas.Remove(type);
        }

        public static bool IsRegistered(Type type)
        {
            return schemas.ContainsKey(type);
        }
    }
}
