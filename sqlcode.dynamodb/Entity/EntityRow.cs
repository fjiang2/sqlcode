using System.Collections.Generic;
using System.Linq;
using sqlcode.dynamodb.extensions;

namespace sqlcode.dynamodb.entity
{
    public class EntityRow : Dictionary<string, EntityValue>
    {
        public EntityRow()
        {
        }

        public override string ToString()
        {
            var dict = this.ToDictionary(x => x.Key, x => x.Value.Value);
            return Json.Serialize(dict);
        }
    }
}
