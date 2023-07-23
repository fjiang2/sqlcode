using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;

namespace sqlcode.dynamodb.Dynamo
{
    public class DynamoDbConnectionStringBuilder : DbConnectionStringBuilder
    {
        public DynamoDbConnectionStringBuilder(string connectionString)
        {
        }

        public DynamoDbConnectionStringBuilder(AWSCredentials connectionString)
        {
        }
    }
}
