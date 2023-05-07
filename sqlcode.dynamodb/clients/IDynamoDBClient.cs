using System;
using System.Data;
using System.Threading.Tasks;

namespace sqlcode.dynamodb.clients
{
    public interface IDynamoDBClient
    {
        Task CreateTable(string tableName, DataColumn partitionKey);
        Task CreateTable(string tableName, DataColumn partitionKey, DataColumn sortKey);
        Task CreateTable(string tableName, string partitionKey, DynamoDataType partitionType);
        Task CreateTable(string tableName, string partitionKey, DynamoDataType partitionType, string sortKey, DynamoDataType sortType);

        Task<bool> Exists(string tableName, TimeSpan timeout);
    }
}