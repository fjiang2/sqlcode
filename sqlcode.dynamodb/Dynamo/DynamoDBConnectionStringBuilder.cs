using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;

namespace sqlcode.dynamodb.Dynamo
{
    public class DynamoDBConnectionStringBuilder : DbConnectionStringBuilder
    {
        public DynamoDBConnectionStringBuilder(AWSCredentials connectionString)
        {
        }

    }
}
