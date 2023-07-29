using System.Data;
using Amazon.DynamoDBv2.Model;
using sqlcode.dynamodb.entities;

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
        Task<EntityTable> ExecuteStatementAsync(string sql);
        Task<bool> Exists(string tableName, TimeSpan timeout);
    }
}