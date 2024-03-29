﻿#define USE_Query_Class
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.Data.SqlClient;

using UnitTestProject.Northwind.dc1;
using Sys.Data.SqlClient;
using Sys.Data.Entity;
using Sys.Data;

namespace UnitTestProject
{
	/// <summary>
	/// Summary description for UnitTestDataContext
	/// </summary>
	[TestClass]
	public class UnitTest_SqlServer_Entity1
	{
		private readonly static string connectionString = Setting.ConnectionString;

#if !USE_Query_Class
		private readonly DataQuery Query;
#endif
		public UnitTest_SqlServer_Entity1()
		{
			DataContext.EntityClassType = EntityClassType.ExtensionClass;

#if USE_Query_Class
			Query.SetDefaultAgent(new SqlDbAgent(new SqlConnectionStringBuilder(connectionString)));
#else
			Query = new DbQuery(connectionString);
#endif
		}



		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void TestMethodSelectIQueryable()
		{
			using (var db = new DbContext(connectionString))
			{
				int id = 1000;
				string name = "Grandma's Boysenberry Spread";
				var table = db.GetTable<Products>();
				var rows = table.Select(row => row.ProductID < id && row.ProductName == name);

				Debug.Assert(rows.First(row => row.ProductID == 6).ProductName == "Grandma's Boysenberry Spread");
			}
		}


		[TestMethod]
		public void TestMethod_Select()
		{
			using (var db = new DbContext(connectionString))
			{
				var table = db.GetTable<Products>();
				var rows = table.Select("ProductID < 1000");

				Debug.Assert(rows.First(row => row.ProductID == 6).ProductName == "Grandma's Boysenberry Spread");
			}
		}

		[TestMethod]
		public void TestMethod_Select_MaxRecords()
		{
			var rows = Query.Select<Products>(string.Empty, new DbLoadOption { StartRecord = 3, MaxRecords = 4, Mode = DbLoadMode.DbFill });
			Debug.Assert(rows.First(row => row.ProductID == 4).ProductName == "Chef Anton's Cajun Seasoning");
			Debug.Assert(rows.Count() == 4);
		}

		[TestMethod]
		public void TestMethodInsert()
		{
			using (var db = new DbContext(connectionString))
			{
				var table = db.GetTable<Products>();
				Products product = new Products
				{
					ProductID = 100,    //identity
					ProductName = "iPhone"
				};

				table.InsertOnSubmit(product);
				string SQL = db.GetNonQueryScript();
				Debug.Assert(SQL.StartsWith("INSERT INTO [Products]([ProductName],[SupplierID],[CategoryID],[UnitPrice],[UnitsInStock],[UnitsOnOrder],[ReorderLevel],[Discontinued]) VALUES(N'iPhone',0,0,0,0,0,0,0)"));
			}
		}

		[TestMethod]
		public void TestMethodUpdate()
		{
			using (var db = new DbContext(connectionString))
			{
				var table = db.GetTable<Products>();
				var product = new
				{
					ProductID = 100,
					ProductName = "iPhone"
				};

				table.PartialUpdateOnSubmit(product);
				string SQL = db.GetNonQueryScript();
				Debug.Assert(SQL.StartsWith("UPDATE [Products] SET [ProductName] = N'iPhone' WHERE [ProductID] = 100"));
			}

			using (var db = new DbContext(connectionString))
			{
				var table = db.GetTable<Products>();
				Products prod = new Products
				{
					ProductID = 200,
					ProductName = "iPhone"
				};
				table.PartialUpdateOnSubmit(prod, row => new { row.ProductID, row.ProductName }, row => row.ProductID == 1);
				string SQL = db.GetNonQueryScript();
				Debug.Assert(SQL.StartsWith("UPDATE [Products] SET [ProductID] = 200, [ProductName] = N'iPhone' WHERE (ProductID = 1)"));
			}
		}

		[TestMethod]
		public void TestMethodInsertOrUpdate()
		{
			using (var db = new DbContext(connectionString))
			{
				var table = db.GetTable<Products>();
				var product = new Products
				{
					ProductID = 100,
					ProductName = "iPhone"
				};

				table.InsertOrUpdateOnSubmit(product);
				string SQL = db.GetNonQueryScript();
				Debug.Assert(SQL.StartsWith("IF EXISTS(SELECT * FROM [Products] WHERE [ProductID] = 100) UPDATE [Products] SET [ProductName] = N'iPhone',[SupplierID] = 0,[CategoryID] = 0,[QuantityPerUnit] = NULL,[UnitPrice] = 0,[UnitsInStock] = 0,[UnitsOnOrder] = 0,[ReorderLevel] = 0,[Discontinued] = 0 WHERE [ProductID] = 100 ELSE INSERT INTO [Products]([ProductName],[SupplierID],[CategoryID],[UnitPrice],[UnitsInStock],[UnitsOnOrder],[ReorderLevel],[Discontinued]) VALUES(N'iPhone',0,0,0,0,0,0,0)"));
			}
		}

		[TestMethod]
		public void TestMethodDelete()
		{
			using (var db = new DbContext(connectionString))
			{
				var table = db.GetTable<Products>();
				var product = new Products
				{
					ProductID = 100,
					ProductName = "iPhone"
				};

				table.DeleteOnSubmit(product);
				string SQL = db.GetNonQueryScript();
				Debug.Assert(SQL.StartsWith("DELETE FROM [Products] WHERE [ProductID] = 100"));
			}
		}

		[TestMethod]
		public void TestMethodSelectOnSubmit()
		{
			using (var db = new DbContext(connectionString))
			{
				var product = db.GetTable<Products>();
				product.SelectOnSubmit(row => row.ProductID == 6);

				var customer = db.GetTable<Customers>();
				customer.SelectOnSubmit(row => row.CustomerID == "MAISD");

				string SQL = db.GetQueryScript();
				Debug.Assert(SQL == "SELECT * FROM [Products] WHERE (ProductID = 6)\r\nSELECT * FROM [Customers] WHERE (CustomerID = 'MAISD')");

				var reader = db.SumbitQueries();
				var L1 = reader.Read<Products>();
				var L2 = reader.Read<Customers>();

				Debug.Assert(L1.First().QuantityPerUnit == "12 - 8 oz jars");
				Debug.Assert(L2.First().PostalCode == "B-1180");

			}
		}

		[TestMethod]
		public void TestMethodSelectOnSubmitChanges()
		{
			using (var db = new DbContext(connectionString))
			{
				db.SelectOnSubmit<Products>(row => row.ProductID == 6);
				db.SelectOnSubmit<Customers>(row => row.CustomerID == "MAISD");

				string SQL = db.GetQueryScript();
				Debug.Assert(SQL == "SELECT * FROM [Products] WHERE (ProductID = 6)\r\nSELECT * FROM [Customers] WHERE (CustomerID = 'MAISD')");

				var reader = db.SumbitQueries();
				var L1 = reader.Read<Products>();
				var L2 = reader.Read<Customers>();

				Debug.Assert(L1.First().QuantityPerUnit == "12 - 8 oz jars");
				Debug.Assert(L2.First().PostalCode == "B-1180");

			}
		}

		[TestMethod]
		public void TestMethodAssociation()
		{
			using (var db = new DbContext(connectionString))
			{
				var order_table = db.GetTable<Orders>();
				var order = order_table.Select(row => row.OrderID == 10256).FirstOrDefault();
				order_table.ExpandOnSubmit<Customers>(order);

				var reader = db.SumbitQueries();
				var L2 = reader.Read<Customers>();

				Debug.Assert(L2.First().CompanyName == "Wellington Importadora");

				var employee = order_table.Expand<Employees>(order).FirstOrDefault();
				Debug.Assert(employee.LastName == "Leverling");
			}
		}

		[TestMethod]
		public void TestMasterDetail()
		{
			using (var db = new DbContext(connectionString))
			{
				var customer_table = db.GetTable<Customers>();
				var customer = customer_table.Select(row => row.CustomerID == "THECR").FirstOrDefault();
				customer_table.ExpandOnSubmit<Orders>(customer);

				var reader = db.SumbitQueries();
				var orders = reader.Read<Orders>();
				var order = orders.FirstOrDefault();

				Debug.Assert(order.ShipName == "The Cracker Box");

				var demographics = customer_table.Expand<CustomerCustomerDemo>(customer).FirstOrDefault();
				Debug.Assert(demographics == null);

				var order_table = db.GetTable<Orders>();
				var shippers = order_table.Expand<Shippers>(order);
				Debug.Assert(shippers.FirstOrDefault().Phone == "(503) 555-3199");

			}
		}

		[TestMethod]
		public void TestMasterDetail2()
		{
			using (var db = new DbContext(connectionString))
			{
				var customers = db.Select<Customers>(row => row.CustomerID == "THECR");
				var customer = customers.FirstOrDefault();

				var orders = db.Expand<Customers, Orders>(customer);
				var order = orders.FirstOrDefault();
				Debug.Assert(order.ShipName == "The Cracker Box");

				var demographics = db.Expand<Customers, CustomerCustomerDemo>(customer);
				var demo = demographics.FirstOrDefault();
				Debug.Assert(demo == null);

				var shippers = db.Expand<Orders, Shippers>(order);
				Debug.Assert(shippers.FirstOrDefault().Phone == "(503) 555-3199");

			}
		}

		[TestMethod]
		public void TestMasterDetail3()
		{
			using (var db = new DbContext(connectionString))
			{
				var customers = db.Select<Customers>(row => row.CustomerID == "THECR");
				var customer = customers.FirstOrDefault();

				db.ExpandOnSubmit<Customers, Orders>(customer);
				db.ExpandOnSubmit<Customers, CustomerCustomerDemo>(customer);

				var reader = db.SumbitQueries();

				var orders = reader.Read<Orders>();
				var order = orders.FirstOrDefault();
				Debug.Assert(order.ShipName == "The Cracker Box");

				var demographics = reader.Read<CustomerCustomerDemo>();
				var demo = demographics.FirstOrDefault();
				Debug.Assert(demo == null);

				db.ExpandOnSubmit<Orders, Shippers>(order);
				reader = db.SumbitQueries();
				var shippers = reader.Read<Shippers>();
				Debug.Assert(shippers.FirstOrDefault().Phone == "(503) 555-3199");

				var L2 = reader.Read<Customers>();
			}
		}

		[TestMethod]
		public void TestEntityExpandAll()
		{
			using (var db = new DbContext(connectionString))
			{
				var customer_table = db.GetTable<Customers>();
				var customer = customer_table.Select(row => row.CustomerID == "THECR").FirstOrDefault();
				Type[] types = customer_table.ExpandOnSubmit(customer);

				var reader = db.SumbitQueries();
				var orders = reader.Read<Orders>();
				var order = orders.FirstOrDefault();
				Debug.Assert(order.ShipName == "The Cracker Box");

				var demographics = reader.Read<CustomerCustomerDemo>();
				var demo = demographics.FirstOrDefault();
				Debug.Assert(demo == null);
			}
		}

		[TestMethod]
		public void TestExpandAllCustomers()
		{
			using (var db = new DbContext(connectionString))
			{
				var customer_table = db.GetTable<Customers>();
				var customers = customer_table.Select(row => row.CustomerID == "THECR" || row.CustomerID == "SUPRD");
				Type[] types = db.ExpandOnSubmit(customers);

				var reader = db.SumbitQueries();
				var orders = reader.Read<Orders>();
				var order = orders.FirstOrDefault(row => row.CustomerID == "THECR");
				Debug.Assert(order.ShipName == "The Cracker Box");

				var demographics = reader.Read<CustomerCustomerDemo>();
				var demo = demographics.FirstOrDefault();
				Debug.Assert(demo == null);
			}
		}


		[TestMethod]
		public void TestExpandAllOnSubmitOrders()
		{
			using (var db = new DbContext(connectionString))
			{
				var order_table = db.GetTable<Orders>();
				var orders = order_table.Select(row => row.OrderID == 10254 || row.OrderID == 10260);

				Type[] types = db.ExpandOnSubmit(orders);
				var reader = db.SumbitQueries();
				var order_details = reader.Read<Order_Details>();

				types = db.ExpandOnSubmit(order_details);
				reader = db.SumbitQueries();
				var products = reader.Read<Products>();

				var product = products.First(row => row.ProductName == "Tarte au sucre");
				Debug.Assert(product.UnitsInStock == 17);

			}
		}

		[TestMethod]
		public void TestExpandAllOrders()
		{
			using (var db = new DbContext(connectionString))
			{
				var orders = db.Select<Orders>(row => row.OrderID == 10254 || row.OrderID == 10260);

				var reader = db.Expand(orders);
				var order_details = reader.Read<Order_Details>();

				reader = db.Expand(order_details);
				var products = reader.Read<Products>();

				var product = products.First(row => row.ProductName == "Tarte au sucre");
				Debug.Assert(product.UnitsInStock == 17);

			}
		}

		[TestMethod]
		public void TestExpandAllOrders2()
		{
			using (var db = new DbContext(connectionString))
			{
				var orders = db.Select<Orders>(row => row.OrderID == 10254 || row.OrderID == 10260);

				var order_details = db.Expand<Orders, Order_Details>(orders);
				var products = db.Expand<Order_Details, Products>(order_details);

				var product = products.First(row => row.ProductName == "Tarte au sucre");
				Debug.Assert(product.UnitsInStock == 17);




				var order = db.Select<Orders>(row => row.OrderID == 10260).First();

				var order_detail = db.Expand<Orders, Order_Details>(order).First();
				products = db.Expand<Order_Details, Products>(order_detail);

				product = products.First(row => row.ProductName == "Jack's New England Clam Chowder");
				Debug.Assert(product.UnitsInStock == 85);
			}

		}


		[TestMethod]
		public void TestQueryExtension()
		{
			var req = new { OrderId = 10254 };
			var orders = Query.Select<Orders>(row => row.OrderID == req.OrderId || row.OrderID == 10260);

			var order_details = Query.Expand<Orders, Order_Details>(orders);
			var products = Query.Expand<Order_Details, Products>(order_details);

			var product = products.First(row => row.ProductName == "Tarte au sucre");
			Debug.Assert(product.UnitsInStock == 17);


			var order = Query.Select<Orders>(row => row.OrderID == 10260).First();

			var order_detail = Query.Expand<Orders, Order_Details>(new Orders[] { order }).First();
			products = Query.Expand<Order_Details, Products>(new Order_Details[] { order_detail });

			product = products.First(row => row.ProductName == "Jack's New England Clam Chowder");
			Debug.Assert(product.UnitsInStock == 85);
		}

		[TestMethod]
		public void TestQueryOperations()
		{
			var demographics = new CustomerDemographics[]
			{
				new CustomerDemographics { CustomerTypeID = "IT",  CustomerDesc = "Computer Science" },
				new CustomerDemographics { CustomerTypeID = "EE",  CustomerDesc = "Electrical Engineering" },
			};

			Query.InsertOrUpdate(demographics);

			Query.InsertOrUpdate(new CustomerCustomerDemo[]
			{
				new CustomerCustomerDemo
				 {
					 CustomerID = "ALFKI",
					 CustomerTypeID = "IT",
				 }
			});

			var desc = Query.Select<CustomerDemographics>(row => row.CustomerTypeID == "IT").First().CustomerDesc;
			Debug.Assert(desc == "Computer Science");

			var customer = Query.Select<CustomerCustomerDemo>(row => row.CustomerTypeID == "IT").First();
			Debug.Assert(customer.CustomerID == "ALFKI");
		}

		[TestMethod]
		public void TestAssoicationClass()
		{
			var query = new DbQuery(connectionString);
			var product = query.Select<Products>(row => row.ProductID == 14).FirstOrDefault();
			var A = product.GetAssociation(query);
			var D = A.Order_Details;

			Debug.Assert(D.Count == 22);
			Debug.Assert(((Categories)A.Category).CategoryName == "Produce");
			Debug.Assert(((Suppliers)A.Supplier).CompanyName == "Mayumi's");
		}

		[TestMethod]
		public void TestRowChangedEvent()
		{
			using (var db = new DbContext(connectionString))
			{
				db.RowChanged += (sender, args) =>
				 {
					 var evt = args.Events.First();
					 Debug.Assert(evt.TypeName == "CustomerDemographics");
					 Debug.Assert(evt.Operation == RowOperation.InsertOrUpdate);
				 };

				var table = db.GetTable<CustomerDemographics>();
				table.InsertOrUpdateOnSubmit(new CustomerDemographics { CustomerTypeID = "IT", CustomerDesc = "Computer Science" });

				db.SubmitChanges();
			}
		}

		[TestMethod]
		public void TestContains1()
		{
			using (var db = new DbContext(connectionString))
			{
				var L = new int[] { 10, 30, 40 }.AsQueryable();
				var table = db.GetTable<Products>();
				table.SelectOnSubmit(row => L.Contains(row.ProductID));

				string SQL = db.GetQueryScript();
				Debug.Assert(SQL == "SELECT * FROM [Products] WHERE ProductID IN (10,30,40)");
			}
		}

		[TestMethod]
		public void TestContains2()
		{
			using (var db = new DbContext(connectionString))
			{
				var L = new int[] { 10 };
				var table = db.GetTable<Products>();
				table.SelectOnSubmit(row => L.Contains(row.ProductID));

				string SQL = db.GetQueryScript();
				Debug.Assert(SQL == "SELECT * FROM [Products] WHERE ProductID IN (10)");
			}
		}

		[TestMethod]
		public void TestContains3()
		{
			using (var db = new DbContext(connectionString))
			{
				var L = new int[] { };
				var table = db.GetTable<Products>();
				table.SelectOnSubmit(row => L.Contains(row.ProductID));

				string SQL = db.GetQueryScript();
				Debug.Assert(SQL == "SELECT * FROM [Products] WHERE ProductID IN ()");
			}
		}


		[TestMethod]
		public void Test2TableContains()
		{
			using (var db = new DbContext(connectionString))
			{
				//"SELECT * FROM [Products] WHERE CategoryID IN (SELECT CategoryID FROM Categories WHERE CategoryName == 'Beverages')"
				var products = Query.Select<Categories, Products>(row => row.CategoryName == "Beverages", row => row.CategoryID, row => row.CategoryID);
				string text = string.Join(",", products.Select(row => row.ProductID));

				Debug.Assert(text == "1,2,24,34,35,38,39,43,67,70,75,76");
			}
		}

		[TestMethod]
		public void Test2DeleteManyRows()
		{
			using (var db = new DbContext(connectionString))
			{
				var products = db.GetTable<Products>();
				products.DeleteOnSubmit(row => row.CategoryID == 1 && row.ProductName == "Apple");
				string SQL = db.GetNonQueryScript();
				Debug.Assert(SQL == "DELETE FROM [Products] WHERE ((CategoryID = 1) AND (ProductName = 'Apple'))");
			}
		}

		[TestMethod]
		public void Test_BulkInsert()
		{
			List<Products> list = new List<Products>
			{
			new Products
			{
				ProductID = 1,
				ProductName = "Chai",
				SupplierID = 1,
				CategoryID = 1,
				QuantityPerUnit = "10 boxes x 20 bags",
				UnitPrice = (decimal)18.0000,
				UnitsInStock = 39,
				UnitsOnOrder = 0,
				ReorderLevel = 10,
				Discontinued = false
			},
			new Products
			{
				ProductID = 2,
				ProductName = "Chang",
				SupplierID = 1,
				CategoryID = 1,
				QuantityPerUnit = "24 - 12 oz bottles",
				UnitPrice = (decimal)19.0000,
				UnitsInStock = 17,
				UnitsOnOrder = 40,
				ReorderLevel = 25,
				Discontinued = false
			},
			new Products
			{
				ProductID = 3,
				ProductName = "Aniseed Syrup",
				SupplierID = 1,
				CategoryID = 2,
				QuantityPerUnit = "12 - 550 ml bottles",
				UnitPrice = (decimal)10.0000,
				UnitsInStock = 13,
				UnitsOnOrder = 70,
				ReorderLevel = 25,
				Discontinued = false
			},
			new Products
			{
				ProductID = 4,
				ProductName = "Chef Anton's Cajun Seasoning",
				SupplierID = 2,
				CategoryID = 2,
				QuantityPerUnit = "48 - 6 oz jars",
				UnitPrice = (decimal)22.0000,
				UnitsInStock = 53,
				UnitsOnOrder = 0,
				ReorderLevel = 0,
				Discontinued = false
			},
			new Products
			{
				ProductID = 5,
				ProductName = "Chef Anton's Gumbo Mix",
				SupplierID = 2,
				CategoryID = 2,
				QuantityPerUnit = "36 boxes",
				UnitPrice = (decimal)21.3500,
				UnitsInStock = 0,
				UnitsOnOrder = 0,
				ReorderLevel = 0,
				Discontinued = true
			},
			new Products
			{
				ProductID = 6,
				ProductName = "Grandma's Boysenberry Spread",
				SupplierID = 3,
				CategoryID = 2,
				QuantityPerUnit = "12 - 8 oz jars",
				UnitPrice = (decimal)25.0000,
				UnitsInStock = 120,
				UnitsOnOrder = 0,
				ReorderLevel = 25,
				Discontinued = false
			},
			new Products
			{
				ProductID = 7,
				ProductName = "Uncle Bob's Organic Dried Pears",
				SupplierID = 3,
				CategoryID = 7,
				QuantityPerUnit = "12 - 1 lb pkgs.",
				UnitPrice = (decimal)30.0000,
				UnitsInStock = 15,
				UnitsOnOrder = 0,
				ReorderLevel = 10,
				Discontinued = false
			},
			new Products
			{
				ProductID = 8,
				ProductName = "Northwoods Cranberry Sauce",
				SupplierID = 3,
				CategoryID = 2,
				QuantityPerUnit = "12 - 12 oz jars",
				UnitPrice = (decimal)40.0000,
				UnitsInStock = 6,
				UnitsOnOrder = 0,
				ReorderLevel = 0,
				Discontinued = false
			},
			new Products
			{
				ProductID = 9,
				ProductName = "Mishi Kobe Niku",
				SupplierID = 4,
				CategoryID = 6,
				QuantityPerUnit = "18 - 500 g pkgs.",
				UnitPrice = (decimal)97.0000,
				UnitsInStock = 29,
				UnitsOnOrder = 0,
				ReorderLevel = 0,
				Discontinued = true
			},
			new Products
			{
				ProductID = 10,
				ProductName = "Ikura",
				SupplierID = 4,
				CategoryID = 8,
				QuantityPerUnit = "12 - 200 ml jars",
				UnitPrice = (decimal)31.0000,
				UnitsInStock = 31,
				UnitsOnOrder = 0,
				ReorderLevel = 0,
				Discontinued = false
			}
			};

			Query.Delete<Products>(row => row.ProductID > 77);
			Query.BulkInsert<Products>(list, batchSize: 4);

			var rows = Query.Select<Products>(row => row.ProductID > 77);
			Debug.Assert(rows.Count() == list.Count);
			
			//clear inserts
			Query.Delete<Products>(row => row.ProductID > 77);
		}

	}
}

