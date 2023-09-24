using System.Reflection.Metadata.Ecma335;
using sqlcode.dynamodb.extensions;

namespace sqlcode.dynamodb.clients.entities
{
    public class EntityValue
    {
        public Type Type { get; set; }
        public object Value { get; set; }

        public EntityValue(object value)
            : this(value.GetType(), value)
        {
        }

        public EntityValue(Type type, object value)
        {
            Type = type;
            Value = value;
        }

        public object this[string name]
        {
            get
            {
                if (Value is IDictionary<string, object> dict)
                {
                    return dict[name];
                }

                throw new InvalidOperationException($"IDictionary<string,object> expected: {Type.FullName}");
            }
            set
            {
                if (Value is IDictionary<string, object> dict)
                {
                    dict[name] = value;
                }

                throw new InvalidOperationException($"IDictionary<string,object> expected: {Type.FullName}");
            }
        }

        public override string ToString()
        {
            if (Value is System.Collections.IEnumerable)
                return Json.Serialize(Value);
            else
                return $"{Value}";
        }
    }
}
