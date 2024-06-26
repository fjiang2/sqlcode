﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.IO;

using UnitTestProject.Northwind.dc1;
using System.Data.SQLite;
using Sys.Data.SQLite;
using Sys.Data.Entity;
using Sys.Data;

namespace UnitTestProject
{
	/// <summary>
	/// Summary description for UnitTestDataContext
	/// </summary>
	[TestClass]
	public class UnitTest_SQLite_Entity1
	{
		private readonly string PATH_PROJECT = Path.GetFullPath("..\\..\\..");
		private readonly string connectionString;
		private readonly DataQuery Query;

		public UnitTest_SQLite_Entity1()
		{
			string fileName = Path.Combine(PATH_PROJECT, "db\\Northwind.db");
			this.connectionString = $"provider=sqlite;Data Source={fileName};Version=3; DateTimeFormat=Ticks; Pooling=True; Max Pool Size=100;";

			DataContext.EntityClassType = EntityClassType.ExtensionClass;
			Query = new DbQuery(connectionString);
		}

		//[TestMethod]
		public void InsertAllRows()
		{
			string[] lines = File.ReadAllLines(Path.Combine(PATH_PROJECT, "db\\sqlite-northwind-insert.sql"));
			foreach (string line in lines)
			{
				if (line == "GO")
					continue;

				Query.Access(new SqlUnit(line)).ExecuteNonQuery();
			}
		}


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
		public void TestMethodSelect()
		{
			using (var db = new DbContext(connectionString))
			{
				var table = db.GetTable<Products>();
				var rows = table.Select("ProductID < 1000");

				Debug.Assert(rows.First(row => row.ProductID == 6).ProductName == "Grandma's Boysenberry Spread");
			}
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
				Debug.Assert(SQL.StartsWith("INSERT INTO [Products]([ProductName],[SupplierID],[CategoryID],[UnitPrice],[UnitsInStock],[UnitsOnOrder],[ReorderLevel],[Discontinued]) VALUES('iPhone',0,0,0,0,0,0,0)"));
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
				Debug.Assert(SQL.StartsWith("UPDATE [Products] SET [ProductName] = 'iPhone' WHERE [ProductID] = 100"));
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
				Debug.Assert(SQL.StartsWith("UPDATE [Products] SET [ProductID] = 200, [ProductName] = 'iPhone' WHERE (ProductID = 1)"));
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
				Debug.Assert(SQL.StartsWith("INSERT OR IGNORE INTO [Products]([ProductName],[SupplierID],[CategoryID],[UnitPrice],[UnitsInStock],[UnitsOnOrder],[ReorderLevel],[Discontinued]) VALUES('iPhone',0,0,0,0,0,0,0);UPDATE [Products] SET [ProductName] = 'iPhone',[SupplierID] = 0,[CategoryID] = 0,[QuantityPerUnit] = NULL,[UnitPrice] = 0,[UnitsInStock] = 0,[UnitsOnOrder] = 0,[ReorderLevel] = 0,[Discontinued] = 0 WHERE [ProductID] = 100"));
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

			Query.Upsert(demographics);

			Query.Upsert(new CustomerCustomerDemo[]
			{
				new CustomerCustomerDemo
				 {
					 CustomerID = "ALFKI",
					 CustomerTypeID = "IT",
				 }
			});

			var desc = Query.Select<CustomerDemographics>(row => row.CustomerTypeID == "IT        ").First().CustomerDesc;
			Debug.Assert(desc == "Computer Science");

			var customer = Query.Select<CustomerCustomerDemo>(row => row.CustomerTypeID == "IT        ").First();
			Debug.Assert(customer.CustomerID == "ALFKI");
		}

		[TestMethod]
		public void TestAssoicationClass()
		{
			var product = Query.Select<Products>(row => row.ProductID == 14).FirstOrDefault();
			var A = product.GetAssociation(Query);
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
					Debug.Assert(evt.Operation == RowOperation.Update);
				};

				var table = db.GetTable<CustomerDemographics>();
				table.UpdateOnSubmit(new CustomerDemographics { CustomerTypeID = "IT", CustomerDesc = "Computer Science" });

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
	}
}
