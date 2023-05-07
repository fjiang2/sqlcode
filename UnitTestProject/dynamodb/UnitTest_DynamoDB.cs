using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if NET6_0
using sqlcode.dynamodb.clients;
using sqlcode.dynamodb.entities;

namespace UnitTestProject.dynamodb
{
    [TestClass]
    public class UnitTest_DynamoDB
    {
        [TestMethod]
        public async Task Test_PartiQL()
        {
            DynamoDBClient client = new DynamoDBClient();
            string SQL = "SELECT * FROM \"DeviceList-taiga\" WHERE TenantId='dev'";
            EntityTable rows = await client.ExecuteStatementAsync(SQL);
            DataTable dt = rows.ToDataTable();
        }

        [TestMethod]
        public async Task Test_Query()
        {
            DynamoTableClient client = new DynamoTableClient("DeviceCommand-taiga", "DcsDeviceId", "CommandRequestId");
            EntityTable rows = await client.QueryAsync("001@dcs.devel");
            DataTable dt = rows.ToDataTable();
        }
    }
}
#endif