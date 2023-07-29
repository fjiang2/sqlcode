using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sqlcode.dynamodb.ado;
using Sys.Data.Entity;

namespace Sys.Data.DynamoDb
{
    public class DbContext : DataContext
    {
        public DbContext(string connectionString)
            : this(new DynamoDbConnectionStringBuilder(connectionString))
        {
        }

        public DbContext(DynamoDbConnectionStringBuilder connectionString)
            : base(new DynamoDbAgent(connectionString))
        {
        }

        public DbContext(DynamoDbAgent agent)
            : base(agent)
        {
        }
    }
}
