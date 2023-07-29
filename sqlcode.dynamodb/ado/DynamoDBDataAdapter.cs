using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using sqlcode.dynamodb.clients;

namespace sqlcode.dynamodb.ado
{
    class DynamoDbDataAdapter : DbDataAdapter
    {
        DynamoDbCommand command;
        DynamoDbConnection connection;

        public DynamoDbDataAdapter(DynamoDbCommand command)
        {
            this.command = command;
            this.connection = (DynamoDbConnection)command.Connection!;
        }

        public override int Fill(DataSet dataSet)
        {
            var credentials = connection.ConnecitonStingBuilder.Credentials;
            var region = connection.ConnecitonStingBuilder.Region;
            var amazonDynamoDBClient = new AmazonDynamoDBClient(credentials, region);
            var dbClient = new DbClient(amazonDynamoDBClient);

            string sql = command.CommandText;
            var result = dbClient.ExecuteStatementAsync(sql).Result;

            return 0;
        }
    }
}
