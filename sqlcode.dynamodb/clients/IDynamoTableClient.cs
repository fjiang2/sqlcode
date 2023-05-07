using Amazon.DynamoDBv2.Model;
using sqlcode.dynamodb.entities;

namespace sqlcode.dynamodb.clients
{
    public interface IDynamoTableClient
    {
        bool HasRangeKey { get; }

        Task<DeleteItemResponse> DeleteAsync(EntityRow row);
        Task DeleteAsync(IEnumerable<EntityRow> rows);
        Task<EntityTable> QueryAsync(string partitionKey);
        Task<EntityTable> QueryAsync(string expression, Dictionary<string, AttributeValue> args, Dictionary<string, string> parameters);
        Task<EntityTable> QueryAsync(string partitionKey, string sortKey);
        Task<PutItemResponse> SaveAsync(EntityRow row);
        Task<EntityTable> ScanAsync(string expression, Dictionary<string, AttributeValue> args, Dictionary<string, string> parameters);
    }
}