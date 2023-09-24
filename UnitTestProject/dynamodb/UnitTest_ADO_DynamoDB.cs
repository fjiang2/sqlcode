using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if NET6_0
using sqlcode.dynamodb.ado;

namespace UnitTestProject.dynamodb
{
    [TestClass]
    public class UnitTest_ADO_DynamoDB
    {
        [TestMethod]
        public void Test_DbConnectionStringBuilder()
        {
            string connectionString = "accessKey=XXXXXI;secretKey=yyyyqs;region=USEast1;";
            var connection = new DynamoDbConnectionStringBuilder(connectionString);

            Assert.AreEqual("XXXXXI", connection.Credentials.GetCredentials().AccessKey);
            Assert.AreEqual("yyyyqs", connection.Credentials.GetCredentials().SecretKey);
            Assert.AreEqual("USEast1", connection.Region.SystemName);
        }
    }
}
#endif