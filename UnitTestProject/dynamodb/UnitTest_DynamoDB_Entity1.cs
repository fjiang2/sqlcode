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
            this.connectionString = "Data Source=dynamoDB;Initial Catalog=taiga;accessKey=;secretKey=;region=us-east-1;";

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
            Assert.AreNotEqual(0, devices.Count);
        }
    }

}

#endif
