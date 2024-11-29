using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.Northwind.dc2;
using Sys.Data;
using Sys.Data.SqlRemote;
using Sys.Data.Entity;

namespace UnitTestProject
{
    /// <summary>
    /// SqlWebServer must run before run the test cases of this class
    /// </summary>
    [TestClass]
    public class UnitTest_SqlRemoteHttp
    {
        private readonly string url = "http://localhost/sqlhandler/";
        private readonly SqlRemoteClient dbClient;
        private readonly DataQuery Query;

        public UnitTest_SqlRemoteHttp()
        {
            dbClient = new SqlRemoteClient(url, DbAgentStyle.SqlServer, "Northwind");
            Query = dbClient.Query;
        }

        [TestMethod]
        public void Test_SELECT()
        {
            string SQL = "SELECT * FROM Products";
            var dt = Query.Access(SQL).FillDataTable();

            Debug.Assert(dt.Rows.Count == 77);
        }

        [TestMethod]
        public void Test_SELECT_Parameters()
        {
            string SQL = "SELECT * FROM Products WHERE UnitsInStock > @Number";
            var dt = Query.Access(SQL, new { Number = 20 }).FillDataTable();

            Debug.Assert(dt.Rows.Count == 48);
        }

        [TestMethod]
        public void Test_Query_SELECT()
        {
            var rows = Query.Select<Products>(row => row.UnitsInStock > 20);

            Debug.Assert(rows.Count() == 48);
        }


        [TestMethod]
        public void TestMethodInsert()
        {
            using (var ctx = dbClient.Context)
            {
                var table = ctx.GetTable<Products>();
                Products product = new Products
                {
                    ProductID = 100,    //identity
                    ProductName = "iPhone"
                };

                table.UpdateOnSubmit(product);
                ctx.SubmitChanges();
            }
        }
    }
}
