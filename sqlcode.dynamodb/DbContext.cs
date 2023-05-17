//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Sys.Data.Entity;

//namespace Sys.Data.DynamoDb
//{
//    public class DbContext : DataContext
//    {
//        public DbContext(string connectionString)
//            : this(new SqlConnectionStringBuilder(connectionString))
//        {
//        }

//        public DbContext(SqlConnectionStringBuilder connectionString)
//            : base(new SqlDbAgent(connectionString))
//        {
//        }

//        public DbContext(DynamoDbAgent agent)
//            : base(agent)
//        {
//        }
//    }
//}
