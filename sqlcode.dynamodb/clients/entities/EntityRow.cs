using sqlcode.dynamodb.extensions;

namespace sqlcode.dynamodb.clients.entities
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
