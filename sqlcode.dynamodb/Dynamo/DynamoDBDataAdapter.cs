using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlcode.dynamodb.Dynamo
{
    class DynamoDbDataAdapter : DbDataAdapter
    {
        public DynamoDbDataAdapter(DynamoDbCommand command)
        {
        }
    }
}
