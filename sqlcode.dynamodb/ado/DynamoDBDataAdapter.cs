using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlcode.dynamodb.ado
{
    class DynamoDbDataAdapter : DbDataAdapter
    {
        public DynamoDbDataAdapter(DynamoDbCommand command)
        {
        }

        protected override int Fill(DataSet dataSet, int startRecord, int maxRecords, string srcTable, IDbCommand command, CommandBehavior behavior)
        {
            return base.Fill(dataSet, startRecord, maxRecords, srcTable, command, behavior);
        }
    }
}
