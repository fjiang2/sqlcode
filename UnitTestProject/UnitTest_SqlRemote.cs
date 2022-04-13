using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sys.Data;
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
        public void Test_Redis_SELECT()
        {
            DbQuery query = new DbQuery(redis);
            var dt = query.Access("SELECT * FROM [Categories] WHERE [CategoryID] = 1").FillDataTable();
            Debug.Assert(dt.Rows.Count == 30);
        }
    }
}
