using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using mudu.aws.core;
using mudu.aws.core.clients;

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
            var connectionString = connection.ConnectionStringBuilder;
            IAccount account = connectionString.Account;
            var dbClient = new DbClient(account);

            string SQL = command.CommandText;
            var query =  new DbQuery(dbClient, connectionString.InitialCatalog);

            QueryOption option = new QueryOption
            {
                Editable = true,
                MaxRows = 0,
            };
            var dt = query.FillDataTableAsync(SQL, option).Result;
            dataSet.Tables.Add(dt);
            return dt.Rows.Count;
        }
    }
}
