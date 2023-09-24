using System.Data;
using Amazon.DynamoDBv2.Model;

namespace sqlcode.dynamodb.clients
{
    public interface IDbClient
    {
        ProvisionedThroughput ProvisionedThroughput { get; set; }

        Task<int> BatchWriteAsync(string tableName, IEnumerable<WriteRequest> writeRequests);
        Task<int> BatchWriteAsync(string tableName, string partitionKey, string sortKey, Dictionary<string, string> records);

        Task CreateTable(string tableName, DataColumn partitionKey);
        Task CreateTable(string tableName, DataColumn partitionKey, DataColumn sortKey);
        Task CreateTable(string tableName, string partitionKey, DynamoDataType partitionType);
        Task CreateTable(string tableName, string partitionKey, DynamoDataType partitionType, string sortKey, DynamoDataType sortType);

        Task<List<Dictionary<string, AttributeValue>>> ExecuteStatementAsync(string sql);
        Task<bool> Exists(string tableName, TimeSpan timeout);

        Task<List<Dictionary<string, AttributeValue>>> QueryAsync(string tableName, string expression, Dictionary<string, AttributeValue> args, Dictionary<string, string> parameters);
        Task<List<Dictionary<string, AttributeValue>>> ScanAsync(string tableName, string expression, Dictionary<string, AttributeValue> args, Dictionary<string, string> parameters);

        Task<bool> SaveAsync(string tableName, Dictionary<string, AttributeValue> item);
        Task<bool> DeleteAsync(string tableName, Dictionary<string, AttributeValue> item);
    }
}