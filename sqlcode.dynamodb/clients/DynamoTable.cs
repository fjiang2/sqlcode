using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using sqlcode.dynamodb.entity;

namespace sqlcode.dynamodb.clients
{

    public class DynamoTable
    {
        const string NO_RANGE_KEY = "no-rangeKey";
        private readonly IAmazonDynamoDB dynamoDBClient;

        private readonly string tableName;
        private readonly string hashKey;
        private readonly string rangeKey;

        public DynamoTable(string tableName, string hashKey)
            : this(tableName, hashKey, NO_RANGE_KEY)
        {
        }

        public DynamoTable(string tableName, string hashKey, string rangeKey)
        {
            dynamoDBClient = new AmazonDynamoDBClient();
            this.tableName = tableName;
            this.hashKey = hashKey;
            this.rangeKey = rangeKey;
        }

        public bool HasRangeKey => rangeKey != NO_RANGE_KEY;

        public Task<EntityTable> QueryAsync(string partitionKey)
        {
            var expression = $"#{hashKey} = :{hashKey}";

            var parameters = new Dictionary<string, string>
            {
                [$"#{hashKey}"] = hashKey,
            };
            var args = new Dictionary<string, AttributeValue>
            {
                [$":{hashKey}"] = new AttributeValue { S = partitionKey },
            };

            return QueryAsync(expression, args, parameters);
        }

        public Task<EntityTable> QueryAsync(string partitionKey, string sortKey)
        {
            var expression = $"#{hashKey} = :{hashKey} and #{rangeKey} = :{rangeKey}";
            var parameters = new Dictionary<string, string>
            {
                [$"#{hashKey}"] = hashKey,
                [$"#{rangeKey}"] = rangeKey,
            };

            var args = new Dictionary<string, AttributeValue>
            {
                [$":{hashKey}"] = new AttributeValue { S = partitionKey },
                [$":{rangeKey}"] = new AttributeValue { S = sortKey }
            };

            return QueryAsync(expression, args, parameters);
        }

        public async Task<EntityTable> QueryAsync(string expression, Dictionary<string, AttributeValue> args, Dictionary<string, string> parameters)
        {
            EntityTable rows = new EntityTable();
            Dictionary<string, AttributeValue>? lastKeyEvaluated = null;
            do
            {
                var request = new QueryRequest
                {
                    TableName = tableName,
                    ReturnConsumedCapacity = "TOTAL",
                    KeyConditionExpression = expression,
                    ExpressionAttributeNames = parameters,
                    ExpressionAttributeValues = args,
                    Limit = 100,
                    ExclusiveStartKey = lastKeyEvaluated
                };

                var response = await dynamoDBClient.QueryAsync(request);
                rows = response.Items.MapTable();

                lastKeyEvaluated = response.LastEvaluatedKey;
            } while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);

            return rows;
        }

        public async Task<EntityTable> ScanAsync(string expression, Dictionary<string, AttributeValue> args, Dictionary<string, string> parameters)
        {
            EntityTable rows = new EntityTable();

            Dictionary<string, AttributeValue>? lastEvaluatedKey = null;
            do
            {
                var request = new ScanRequest
                {
                    TableName = tableName,
                    FilterExpression = expression,
                    ExpressionAttributeValues = args,
                    ExpressionAttributeNames = parameters,
                    ExclusiveStartKey = lastEvaluatedKey,
                    Limit = 50,
                };

                var response = await dynamoDBClient.ScanAsync(request);
                lastEvaluatedKey = response.LastEvaluatedKey;

                rows = response.Items.MapTable();
            } while (lastEvaluatedKey.Count != 0);

            return rows;
        }

        public async Task DeleteAsync(IEnumerable<EntityRow> rows)
        {
            foreach (var row in rows)
            {
                var response = await DeleteAsync(row);
            }
        }

        public async Task<DeleteItemResponse> DeleteAsync(EntityRow row)
        {
            var attributes = new Dictionary<string, AttributeValue>
            {
                [hashKey] = new AttributeValue(row[hashKey]?.ToString()),
            };

            if (HasRangeKey)
            {
                attributes.Add(rangeKey, new AttributeValue(row[rangeKey]?.ToString()));
            }

            var response = await dynamoDBClient.DeleteItemAsync(tableName, attributes);
            return response;
        }

        public async Task<PutItemResponse> SaveAsync(EntityRow row)
        {
            var item = new Dictionary<string, AttributeValue>();

            foreach (var kvp in row)
            {
                string key = kvp.Key;
                object? value = kvp.Value;
                if (value is string)
                {
                    item[key] = new AttributeValue((string)value);
                }
                else if (value is IEnumerable<string> values)
                {
                    item[key] = new AttributeValue(values.ToList());
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            var response = await dynamoDBClient.PutItemAsync(tableName, item);
            return response;
        }

    }
}
