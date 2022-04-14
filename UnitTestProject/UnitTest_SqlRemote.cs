using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Net.Http;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject.Northwind.dc2;
using Sys.Data;
using Sys.Data.SqlRemote;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest_SqlRemote
    {
        string url = "http://localhost/sqlhandler/";
        SqlRemoteAgent agent;
        DbQuery query;

        public UnitTest_SqlRemote()
        {
            SqlHttpClient client = new SqlHttpClient(url)
            {
                Style = DbAgentStyle.SqlServer,
            };

            agent = new SqlRemoteAgent(client);
            query = new DbQuery(agent);

        }

        [TestMethod]
        public void Test_SELECT()
        {
            string SQL = "SELECT * FROM Products";
            var dt = query.Access(SQL).FillDataTable();

            Debug.Assert(dt.Rows.Count == 77);
        }


        [TestMethod]
        public void Test_Query_SELECT()
        {
            var rows = query.Select<Products>(row => row.UnitsInStock > 20);

            Debug.Assert(rows.Count() == 48);
        }


        [TestMethod]
        public void TestMethodInsert()
        {
            using (var db = new DbContext(agent))
            {
                var table = db.GetTable<Products>();
                Products product = new Products
                {
                    ProductID = 100,    //identity
                    ProductName = "iPhone"
                };

                table.UpdateOnSubmit(product);
                db.SubmitChanges();
            }
        }
    }
}
