using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.Data.Entity;
using Sys.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if NET7_0
using Sys.Data.DynamoDb;

namespace UnitTestProject.DynamoDB
{
    [TestClass]
    public class UnitTest_DynamoDB_Entity1
    {
        private readonly string connectionString;
        private readonly DataQuery Query;

        public UnitTest_DynamoDB_Entity1()
        {
            this.connectionString = File.ReadAllText(@"c:\local\settings\dynamodb.connection.string.txt");

            DataContext.EntityClassType = EntityClassType.ExtensionClass;
            Query = new DbQuery(connectionString);
        }

        [TestMethod]
        public void TestMethodSelectOnSubmitChanges()
        {
            string tenantId = "dev";
            string SQL = $"SELECT * FROM DeviceList WHERE TenantId='{tenantId}'";
            var access = Query.Access(SQL);
            var devices = access.FillDataColumn<string>("DeviceId");
        }
    }

}

#endif
