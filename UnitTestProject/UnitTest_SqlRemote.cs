using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Net.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sys.Data;
using Sys.Data.SqlRemote;
using Sys.Data.SqlRedis;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest_SqlRemote
    {
        const string redis = "Provider=Redis;data source=localhost:6379;";

        public UnitTest_SqlRemote()
        {
            var agent = new Sys.Data.SqlClient.SqlDbAgent(new SqlConnectionStringBuilder(Setting.ConnectionString));
        }

        [TestMethod]
        public void Test_Http_SELECT()
        {
            string url = "http://localhost/sqlhandler/";
            SqlHttpClient client = new SqlHttpClient(url);

            string SQL = "SELECT * FROM Products";
            SqlRemoteAccess access = new SqlRemoteAccess(client, new SqlUnit(SQL));
            var dt = access.FillDataTable();
            Debug.Assert(dt.Rows.Count == 77);
        }
    }
}
