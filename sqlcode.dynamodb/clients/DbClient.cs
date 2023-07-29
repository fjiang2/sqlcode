using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using sqlcode.dynamodb.entities;

namespace sqlcode.dynamodb.clients
{
    public class DbClient : IDbClient
    {
        private readonly IAmazonDynamoDB dynamoDBClient;
        public ProvisionedThroughput ProvisionedThroughput { get; set; } = new ProvisionedThroughput
        {
            ReadCapacityUnits = 1,
            WriteCapacityUnits = 1,
        };

        public DbClient()
        {
            this.dynamoDBClient = new AmazonDynamoDBClient();
        }

        public DbClient(IAmazonDynamoDB dynamoDBClient)
        {
            this.dynamoDBClient = dynamoDBClient;
        }

        public async Task<bool> Exists(string tableName, TimeSpan timeout)
        {
            DateTime startTime = DateTime.Now;

            while (DateTime.Now - startTime < timeout)
            {
                Thread.Sleep(1000);

                ListTablesResponse tables = await dynamoDBClient.ListTablesAsync();
                if (tables.TableNames.Contains(tableName))
                    return true;
            }

            return false;
        }

        public Task CreateTable(string tableName, string partitionKey, DynamoDataType partitionType, string sortKey, DynamoDataType sortType)
        {
            return dynamoDBClient.CreateTableAsync(new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = partitionKey,
                        AttributeType = partitionType.ToString(),
                    },
                    new AttributeDefinition
                    {
                        AttributeName = sortKey,
                        AttributeType = sortType.ToString(),
                    },
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement    //partition key
                    {
                        AttributeName = partitionKey,
                        KeyType = "HASH",
                    },
                    new KeySchemaElement    //sort key
                    {
                        AttributeName = sortKey,
                        KeyType = "RANGE",
                    },
                },
                ProvisionedThroughput = ProvisionedThroughput,
            });
        }

        public Task CreateTable(string tableName, string partitionKey, DynamoDataType partitionType)
        {
            return dynamoDBClient.CreateTableAsync(new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = partitionKey,
                        AttributeType = partitionType.ToString(),
                    },
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement    //partition key
                    {
                        AttributeName = partitionKey,
                        KeyType = "HASH",
                    },
                },
                ProvisionedThroughput = ProvisionedThroughput,
            });
        }

        public Task CreateTable(string tableName, DataColumn partitionKey, DataColumn sortKey)
        {
            return dynamoDBClient.CreateTableAsync(new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = partitionKey.ColumnName,
                        AttributeType = GetAttributeType(partitionKey.DataType),
                    },
                    new AttributeDefinition
                    {
                        AttributeName = sortKey.ColumnName,
                        AttributeType = GetAttributeType(sortKey.DataType),
                    },
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement    //partition key
                    {
                        AttributeName = partitionKey.ColumnName,
                        KeyType = "HASH",
                    },
                    new KeySchemaElement    //sort key
                    {
                        AttributeName =sortKey.ColumnName,
                        KeyType = "RANGE",
                    },
                },
                ProvisionedThroughput = ProvisionedThroughput,
            });
        }

        public Task CreateTable(string tableName, DataColumn partitionKey)
        {
            return dynamoDBClient.CreateTableAsync(new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = partitionKey.ColumnName,
                        AttributeType = GetAttributeType(partitionKey.DataType),
                    },
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement    //partition key
                    {
                        AttributeName = partitionKey.ColumnName,
                        KeyType = "HASH",
                    },
                },
                ProvisionedThroughput = ProvisionedThroughput,
            });
        }

        private static string GetAttributeType(Type type)
        {
            if (type == typeof(string))
                return "S"; //string
            else if (type == typeof(int))
                return "N"; //number
            else
                return "B"; //binary
        }

        /// <summary>
        /// Write records/items into table
        /// </summary>
        /// <param name="tableName">DynamoDB table name</param>
        /// <param name="writeRequests"></param>
        /// <returns>Total number of records saved</returns>
        public async Task<int> BatchWriteAsync(string tableName, IEnumerable<WriteRequest> writeRequests)
        {
            //AWS: Member must have length <= 25, Member must have length >= 1
            const int MAX_BATCH_SIZE = 25;
            const int MAX_RETRY_NUMBER = 5;

            int count = 0;
            foreach (var chunk in writeRequests.Chunk(MAX_BATCH_SIZE))
            {
                var requestItems = new Dictionary<string, List<WriteRequest>>
                {
                    { tableName, chunk.ToList() },
                };

                BatchWriteItemRequest request = new BatchWriteItemRequest
                {
                    ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL,
                    RequestItems = requestItems,
                };

                int retry = 0;
                bool success = false;
                while (retry++ < MAX_RETRY_NUMBER && !success)
                {
                    try
                    {
                        BatchWriteItemResponse batchWriteItemResponse;
                        do
                        {
                            batchWriteItemResponse = await dynamoDBClient.BatchWriteItemAsync(request);
                            success = batchWriteItemResponse.HttpStatusCode == HttpStatusCode.OK;
                            if (success)
                            {
                                foreach (var tableConsumedCapacity in batchWriteItemResponse.ConsumedCapacity)
                                {
                                    count += Convert.ToInt32(tableConsumedCapacity.CapacityUnits);
                                }
                            }
                            else
                            {
                                Console.Error.WriteLine($"Retry {retry}: Error in BatchWriteAsync to table {tableName}, status={batchWriteItemResponse.HttpStatusCode}");
                            }

                            request.RequestItems = batchWriteItemResponse.UnprocessedItems;

                        } while (batchWriteItemResponse.UnprocessedItems.Count > 0);
                    }
                    catch (ItemCollectionSizeLimitExceededException ex)
                    {
                        Console.Error.WriteLine($"Item size limit exceeded in BatchWriteAsync to table {tableName}, {ex.Message}");
                        break;
                    }
                    catch (ProvisionedThroughputExceededException ex)
                    {
                        // If none of the items can be processed due to insufficient provisioned throughput
                        Console.Error.WriteLine($"Retry {retry}: Provisioned throughput exceeded in BatchWriteAsync to table {tableName}, {ex.Message}");

                        // Exponential backoff
                        await Task.Delay(100 * (1 >> retry));
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Retry {retry}: Error in BatchWriteAsync to table {tableName}, {ex.Message}");
                        await Task.Delay(100);
                    }
                }
            }

            return count;
        }


        /// <summary>
        /// Batch save records/items into table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="records"></param>
        /// <returns>Total number of records saved</returns>
        public async Task<int> BatchWriteAsync(string tableName, string partitionKey, string sortKey, Dictionary<string, string> records)
        {
            var writeRequests = new List<WriteRequest>();

            foreach (var record in records)
            {
                var item = new Dictionary<string, AttributeValue>
                {
                    [partitionKey] = new AttributeValue(record.Key),
                    [sortKey] = new AttributeValue(record.Value),
                };

                WriteRequest putRequest = new WriteRequest(new PutRequest(item));
                writeRequests.Add(putRequest);
            }

            return await BatchWriteAsync(tableName, writeRequests);
        }

        /// <summary>
        /// Execute PartiQL, e.g SELECT * FROM "DeviceList-rainforest" WHERE TenantId="devel"
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<EntityTable> ExecuteStatementAsync(string sql)
        {
            //PartiQL
            ExecuteStatementRequest request = new ExecuteStatementRequest
            {
                Statement = sql,
                ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL
            };

            ExecuteStatementResponse response = await dynamoDBClient.ExecuteStatementAsync(request);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                var rows = response.Items.MapTable();
                return rows;
            }

            return new EntityTable();
        }
    }
}
