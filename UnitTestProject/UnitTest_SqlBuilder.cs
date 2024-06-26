﻿#define HAS_SQL_SERVER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sys.Data;
using Sys.Data.Text;
using Sys.Data.SqlClient;
using UnitTestProject.Northwind.dc1;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest_SqlBuilder
	{
		private readonly DbAgentStyle SQLite = DbAgentStyle.SQLite;
		private readonly SqlConnectionStringBuilder conn;

		private readonly Expression ProductId = "ProductId".AsColumn();
		private readonly string Products = "Products";
		private readonly ITableName Categories = "Categories".AsTableName();

		public UnitTest_SqlBuilder()
		{
			conn = new SqlConnectionStringBuilder(Setting.ConnectionString);
		}

		[TestMethod]
		public void Test_SELECT()
		{

			var CategoryID = "CategoryID".AsColumn();

			string SQL = new SqlBuilder()
				.SELECT().TOP(10).COLUMNS().FROM("dbo.Categories").WHERE(CategoryID >= 10)
				.ToString();

			Debug.Assert(SQL == "SELECT TOP 10 * FROM [Categories] WHERE [CategoryID] >= 10");

			SQL = new SqlBuilder()
				.SELECT().DISTINCT().COLUMNS(CategoryID).FROM("dbo.[Products]").WHERE(CategoryID >= 2)
				.ToString();

			Debug.Assert(SQL == "SELECT DISTINCT [CategoryID] FROM [Products] WHERE [CategoryID] >= 2");
		}

		[TestMethod]
		public void Test_JOIN1()
		{
			string SQL = new SqlBuilder()
				.SELECT().COLUMNS("CategoryName".AsColumn("C"), "*".AsColumn("P"))
				.FROM("Products", "P")
				.INNER().JOIN("Categories", "C").ON("CategoryID".AsColumn("C") == "CategoryID".AsColumn("P"))
				.WHERE("CategoryName".AsColumn("C") == "Dairy Products")
				.ToString();

			Debug.Assert(SQL == "SELECT C.[CategoryName], P.* FROM [Products] P INNER JOIN [Categories] C ON C.[CategoryID] = P.[CategoryID] WHERE C.[CategoryName] = N'Dairy Products'");

		}

		[TestMethod]
		public void Test_JOIN2()
		{
			string sql = @"SELECT [Categories].[CategoryName], [Products].[ProductName], [Products].[QuantityPerUnit], [Products].[UnitsInStock], [Products].[Discontinued]
FROM [Categories]
INNER JOIN [Products] ON [Categories].[CategoryID] = [Products].[CategoryID]
WHERE [Products].[Discontinued] <> 1";

			string query = new SqlBuilder()
				.SELECT()
				.COLUMNS(
					"CategoryName".AsColumn(Categories),
					"ProductName".AsColumn(Products),
					"QuantityPerUnit".AsColumn(Products),
					"UnitsInStock".AsColumn(Products),
					"Discontinued".AsColumn(Products)
					)
				.AppendLine()
				.FROM(Categories)
				.AppendLine()
				.INNER().JOIN(Products).ON("CategoryID".AsColumn(Categories) == "CategoryID".AsColumn(Products))
				.AppendLine()
				.WHERE("Discontinued".AsColumn(Products) != 1)
				.ToString();

			Debug.Assert(sql == query);
		}


		[TestMethod]
		public void Test_Column_AS_Name()
		{
			string SQL = new SqlBuilder()
				.SELECT().COLUMNS("ProductID".AsColumn().AS("Id"), "ProductName".AsColumn().AS("Name"))
				.FROM("Products")
				.ToString();

			Debug.Assert(SQL == "SELECT [ProductID] AS Id, [ProductName] AS Name FROM [Products]");

			SQL = new SqlBuilder()
				.SELECT().COLUMNS("ProductID".AsColumn().AS("Id"), "ProductName".AsColumn().AS("Name"))
				.FROM("Products")
				.ToString();

			Debug.Assert(SQL == "SELECT [ProductID] AS Id, [ProductName] AS Name FROM [Products]");
		}

		[TestMethod]
		public void Test_BETWEEN_IN()
		{
			var ProductID = "ProductID".AsColumn();
			string SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS()
				.FROM("Products")
				.WHERE(ProductID.IN(1, 2, 3, 4).OR(ProductID.BETWEEN(1, 4)))
				.ToString();

			Debug.Assert(SQL == "SELECT * FROM [Products] WHERE ([ProductID] IN (1, 2, 3, 4)) OR ([ProductID] BETWEEN 1 AND 4)");
		}

		[TestMethod]
		public void Test_INSERT()
		{
			string SQL = new SqlBuilder()
				.INSERT_INTO("Categories", new string[] { "CategoryName", "Description", "Picture" })
				.VALUES("Seafood", "Seaweed and fish", new byte[] { 0x15, 0xC2 })
				.ToString();

			Debug.Assert(SQL == "INSERT INTO [Categories] ([CategoryName], [Description], [Picture]) VALUES (N'Seafood', N'Seaweed and fish', 0x15C2)");

			SQL = new SqlBuilder()
				.INSERT_INTO("Categories", new Expression[] { "CategoryName".AsColumn(), "Description".AsColumn(), "Picture".AsColumn() })
				.VALUES("Seafood", "Seaweed and fish", new byte[] { 0x15, 0xC2 })
				.ToString();

			Debug.Assert(SQL == "INSERT INTO [Categories] ([CategoryName], [Description], [Picture]) VALUES (N'Seafood', N'Seaweed and fish', 0x15C2)");

		}

		[TestMethod]
		public void Test_SQLite_INSERT()
		{
			string SQL = new SqlBuilder()
				.INSERT_INTO("Categories", new string[] { "CategoryName", "Description", "Picture" })
				.VALUES("Seafood", "Seaweed and fish", new byte[] { 0x15, 0xC2 })
				.ToString(SQLite);

			Debug.Assert(SQL == "INSERT INTO [Categories] ([CategoryName], [Description], [Picture]) VALUES ('Seafood', 'Seaweed and fish', x'15C2')");
		}

		[TestMethod]
		public void Test_INSERT_IDENTIY()
		{
			var context = new ParameterContext();

			string SQL = new SqlBuilder()
				.DELETE_FROM(Categories).WHERE("CategoryName".AsColumn() == "Electronics")
				.ToString();


			Debug.Assert(SQL == @"DELETE FROM [Categories] WHERE [CategoryName] = N'Electronics'");

#if HAS_SQL_SERVER
			int result = new DbQuery(conn).Access(SQL, context.Parameters).ExecuteNonQuery();
			Debug.Assert(result >= 0);
#endif

			SQL = new SqlBuilder()
				.INSERT_INTO("Categories", new string[] { "CategoryName", "Description", "Picture" })
				.VALUES("Electronics", "Electronics and Computers", new byte[] { 0x15, 0xC2 })
				.AppendLine()
				.SET(context.AsOutParameter("CategoryId", 0) == Expression.IDENTITY)
				.ToString();


			Debug.Assert(SQL == @"INSERT INTO [Categories] ([CategoryName], [Description], [Picture]) VALUES (N'Electronics', N'Electronics and Computers', 0x15C2)
SET @CategoryId = @@IDENTITY");

#if HAS_SQL_SERVER
			result = new DbQuery(conn).Access(SQL, context.Parameters).ExecuteNonQuery();
			Debug.Assert(result == 1);

			int CategoryId = (int)context["CategoryId"].Value;
			Debug.Assert(CategoryId > 8);
#endif
		}

		[TestMethod]
		public void Test_UPDATE1()
		{
			string SQL = new SqlBuilder()
				.UPDATE("Categories")
				.SET(
					"CategoryName".AssignColumn("Seafood"),
					"Description".AssignColumn("Seaweed and fish"),
					"Picture".AssignColumn(new byte[] { 0x15, 0xC2 })
					)
				.WHERE("CategoryID".AsColumn() == 8)
				.ToString();

			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = N'Seafood', [Description] = N'Seaweed and fish', [Picture] = 0x15C2 WHERE [CategoryID] = 8");

			SQL = new SqlBuilder()
				.UPDATE("Categories")
				.SET(
					"CategoryName".AsColumn().LET("Seafood"),
					"Description".AsColumn().LET("Seaweed and fish"),
					Expression.LET("Picture".AsColumn(), new byte[] { 0x15, 0xC2 })
					)
				.WHERE("CategoryID".AsColumn() == 8)
				.ToString();

			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = N'Seafood', [Description] = N'Seaweed and fish', [Picture] = 0x15C2 WHERE [CategoryID] = 8");
		}

		[TestMethod]
		public void Test_UPDATE2()
		{
			string sql = "UPDATE [Products] SET [ProductName] = N'Apple', [UnitPrice] = 20 WHERE [ProductId] BETWEEN 10 AND 30";
			string query = new SqlBuilder()
				.UPDATE(Products)
				.SET("ProductName".AsColumn() == "Apple", "UnitPrice".AsColumn() == 20)
				.WHERE(ProductId.BETWEEN(10, 30))
				.ToString();

			Debug.Assert(sql == query);
		}

		[TestMethod]
		public void Test_SQLite_UPDATE2()
		{
			string sql = "UPDATE [Products] SET [ProductName] = 'Apple', [UnitPrice] = 20 WHERE [ProductId] BETWEEN 10 AND 30";
			string query = new SqlBuilder()
				.UPDATE(Products)
				.SET("ProductName".AsColumn() == "Apple", "UnitPrice".AsColumn() == 20)
				.WHERE(ProductId.BETWEEN(10, 30))
				.ToString(SQLite);

			Debug.Assert(sql == query);
		}

		[TestMethod]
		public void Test_DELETE()
		{
			string SQL = new SqlBuilder()
				.DELETE_FROM("Categories")
				.WHERE("CategoryID".AsColumn() == 8)
				.ToString();

			Debug.Assert(SQL == "DELETE FROM [Categories] WHERE [CategoryID] = 8");
		}

		[TestMethod]
		public void Test_IS_NOT_EXISTS()
		{
			string Categories = "Categories";

			var select = new SqlBuilder()
				.SELECT()
				.COLUMNS()
				.FROM(Categories)
				.WHERE("CategoryID".AsColumn() == 8);

			var insert = new SqlBuilder()
				.INSERT_INTO(Categories, new string[] { "CategoryName", "Description", "Picture" })
				.VALUES("Seafood", "Seaweed and fish", new byte[] { 0x15, 0xC2 });

			var update = new SqlBuilder()
				.UPDATE(Categories)
				.SET(
					"CategoryName".AssignColumn("Seafood"),
					"Description".AssignColumn("Seaweed and fish"),
					"Picture".AssignColumn(new byte[] { 0x15, 0xC2 })
					)
				.WHERE("CategoryID".AsColumn() == 8);

			string SQL = new Statement()
				.IF(select.EXISTS().NOT(), insert, update)
				.ToString();

			Debug.Assert(SQL == "IF NOT EXISTS (SELECT * FROM [Categories] WHERE [CategoryID] = 8) INSERT INTO [Categories] ([CategoryName], [Description], [Picture]) VALUES (N'Seafood', N'Seaweed and fish', 0x15C2) ELSE UPDATE [Categories] SET [CategoryName] = N'Seafood', [Description] = N'Seaweed and fish', [Picture] = 0x15C2 WHERE [CategoryID] = 8");

			SQL = new Statement()
				.IF(select.EXISTS(), insert, update)
				.ToString();

			Debug.Assert(SQL == "IF EXISTS (SELECT * FROM [Categories] WHERE [CategoryID] = 8) INSERT INTO [Categories] ([CategoryName], [Description], [Picture]) VALUES (N'Seafood', N'Seaweed and fish', 0x15C2) ELSE UPDATE [Categories] SET [CategoryName] = N'Seafood', [Description] = N'Seaweed and fish', [Picture] = 0x15C2 WHERE [CategoryID] = 8");

		}

		[TestMethod]
		public void Test_GROUP_BY()
		{
			var SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS("CategoryID".AsColumn(), Expression.COUNT_STAR)
				.FROM("Products")
				.GROUP_BY("CategoryID")
				.HAVING(Expression.COUNT_STAR > 10)
				.ToString();

			Debug.Assert(SQL == "SELECT [CategoryID], COUNT(*) FROM [Products] GROUP BY [CategoryID] HAVING COUNT(*) > 10");
		}

		[TestMethod]
		public void Test_ORDER_BY()
		{
			var SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS()
				.FROM("Products")
				.WHERE("UnitPrice".AsColumn() > 50.0)
				.ORDER_BY("ProductName").DESC()
				.ToString();

			Debug.Assert(SQL == "SELECT * FROM [Products] WHERE [UnitPrice] > 50 ORDER BY [ProductName] DESC");
		}

		[TestMethod]
		public void Test_CASE_WHEN()
		{
			var case_when = Expression.CASE(
				"CategoryID".AsColumn(),
				new Expression[]
				{
					Expression.WHEN(1, "Road"),
					Expression.WHEN(2, "Mountain"),
					Expression.WHEN(3, "Touring"),
					Expression.WHEN(4, "Other sale items"),
				},
				"Not for sale");

			var SQL = new SqlBuilder()
				.SELECT().TOP(10)
				.COLUMNS("ProductID".AsColumn(), "Category".AssignColumn(case_when), "ProductName".AsColumn())
				.FROM("Products")
				.ORDER_BY("ProductID")
				.ToString();

			Debug.Assert(SQL == "SELECT TOP 10 [ProductID], [Category] = CASE [CategoryID] WHEN 1 THEN N'Road' WHEN 2 THEN N'Mountain' WHEN 3 THEN N'Touring' WHEN 4 THEN N'Other sale items' ELSE N'Not for sale' END, [ProductName] FROM [Products] ORDER BY [ProductID]");

			SQL = new SqlBuilder()
				.SELECT().TOP(10)
				.COLUMNS("ProductID".AsColumn(),
					"Category".AsColumn()
						.LET()
						.CASE("CategoryID".AsColumn())
						.WHEN(1).THEN("Road")
						.WHEN(2).THEN("Mountain")
						.WHEN(3).THEN("Touring")
						.WHEN(4).THEN("Other sale items")
						.ELSE("Not for sale")
						.END()
					,
					"ProductName".AsColumn())
				.FROM("Products")
				.ORDER_BY("ProductID")
				.ToString();

			Debug.Assert(SQL == "SELECT TOP 10 [ProductID], [Category] = CASE [CategoryID] WHEN 1 THEN N'Road' WHEN 2 THEN N'Mountain' WHEN 3 THEN N'Touring' WHEN 4 THEN N'Other sale items' ELSE N'Not for sale' END, [ProductName] FROM [Products] ORDER BY [ProductID]");
		}

		[TestMethod]
		public void Test_PARAMETER()
		{
			ParameterContext context = new ParameterContext();
			var SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS()
				.FROM("Orders")
				.WHERE("OrderDate".AsColumn() <= Expression.GETDATE & "ShipCity".AsColumn() == "London" & "EmployeeID".AsColumn() == context.AsParameter("Id", value: 7))
				.ToString();

			Debug.Assert(SQL == "SELECT * FROM [Orders] WHERE (([OrderDate] <= GETDATE()) AND ([ShipCity] = N'London')) AND ([EmployeeID] = @Id)");

#if HAS_SQL_SERVER

			var dt1 = new DbQuery(conn).Access(SQL, new { Id = 7 }).FillDataTable();
			Debug.Assert(dt1.Rows.Count >= 5);

			var dt2 = new DbQuery(conn).Access(SQL, context.Parameters).FillDataTable();
			Debug.Assert(dt2.Rows.Count >= 5);

			//if you want to change value of parameter from 7 to 10
			context["Id"].Value = 10;

			dt2 = new DbQuery(conn).Access(SQL, context.Parameters).FillDataTable();
			Debug.Assert(dt2.Rows.Count == 0);

#endif

		}

		[TestMethod]
		public void Test_AND_OR()
		{
			var where1 = new Expression[]
			{
				"OrderDate".AsColumn() <= Expression.GETDATE,
				"ShipCity".AsColumn() == "London",
				"EmployeeID".AsColumn() ==  7
			}.AND();

			var where2 = Expression.AND(
				"OrderDate".AsColumn() <= Expression.GETDATE,
				"ShipCity".AsColumn() == "London",
				"EmployeeID".AsColumn() == 7);

			Debug.Assert(where1.ToString() == "([OrderDate] <= GETDATE()) AND ([ShipCity] = N'London') AND ([EmployeeID] = 7)");
			Debug.Assert(where1.ToString() == where2.ToString());

			var SQL = new SqlBuilder()
			.SELECT()
			.COLUMNS()
			.FROM("Orders")
			.WHERE(new Expression[]
			{
				"OrderDate".AsColumn() <= Expression.GETDATE,
				"ShipCity".AsColumn() == "London",
				"EmployeeID".AsColumn() ==  7
			}.AND())
			.ToString();

			Debug.Assert(SQL == "SELECT * FROM [Orders] WHERE ([OrderDate] <= GETDATE()) AND ([ShipCity] = N'London') AND ([EmployeeID] = 7)");

			var _and = ("OrderDate".AsColumn() <= Expression.GETDATE).AND("EmployeeID".AsColumn() == 7);
			Debug.Assert(_and.ToString() == "([OrderDate] <= GETDATE()) AND ([EmployeeID] = 7)");

			var _or = ("OrderDate".AsColumn() <= Expression.GETDATE).OR("EmployeeID".AsColumn() == 7);
			Debug.Assert(_or.ToString() == "([OrderDate] <= GETDATE()) OR ([EmployeeID] = 7)");
		}

		[TestMethod]
		public void Test_Function()
		{
			var min_price = "MIN".AsFunction("UnitPrice".AsColumn());
			var where = ("CategoryId".AsColumn() == 5).AND("UnitsInStock".AsColumn() > 100);

			var SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS(min_price)
				.FROM("Products")
				.WHERE(where).
				ToString();

			Debug.Assert(SQL == "SELECT MIN([UnitPrice]) FROM [Products] WHERE ([CategoryId] = 5) AND ([UnitsInStock] > 100)");

			SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS("UnitPrice".AsColumn().MIN())
				.FROM("Products")
				.WHERE(where).
				ToString();

			Debug.Assert(SQL == "SELECT MIN([UnitPrice]) FROM [Products] WHERE ([CategoryId] = 5) AND ([UnitsInStock] > 100)");

			SQL = new SqlBuilder().SELECT().COLUMNS("COUNT".AsFunction("*".AsColumn())).FROM("Products").ToString();
			Debug.Assert(SQL == "SELECT COUNT(*) FROM [Products]");

		}

		[TestMethod]
		public void Test_IN_SELECT()
		{
			var ProductId = "ProductId".AsColumn();
			var CategoryId = "CategoryId".AsColumn();

			var select = new SqlBuilder()
				.SELECT()
				.COLUMNS(ProductId)
				.FROM("Products")
				.WHERE(CategoryId == 7);


			var SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS()
				.FROM("Order Details")
				.WHERE(ProductId.IN(select)).
				ToString();

			Debug.Assert(SQL == "SELECT * FROM [Order Details] WHERE [ProductId] IN (SELECT [ProductId] FROM [Products] WHERE [CategoryId] = 7)");

		}


		[TestMethod]
		public void Test_IN_List()
		{
			var ProductId = "ProductId".AsColumn();
			List<int> list = new List<int> { 1, 2, 3, 4, 5 };

			var SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS()
				.FROM("Products")
				.WHERE(ProductId.IN(list)).
				ToString();

			Debug.Assert(SQL == "SELECT * FROM [Products] WHERE [ProductId] IN (1, 2, 3, 4, 5)");

		}


		[TestMethod]
		public void Test_NOT_IN_List()
		{
			var ProductId = "ProductId".AsColumn();
			List<int> list = new List<int> { 1, 2, 3, 4, 5 };

			var SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS()
				.FROM("Products")
				.WHERE(ProductId.NOT_IN(list)).
				ToString();

			Debug.Assert(SQL == "SELECT * FROM [Products] WHERE [ProductId] NOT IN (1, 2, 3, 4, 5)");

		}


		[TestMethod]
		public void Test_IS_NOT_NULL1()
		{
			var Description = "Description".AsColumn();
			var CategoryId = "CategoryId".AsColumn();

			var SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS()
				.FROM("Categories")
				.WHERE(CategoryId.IS_NOT_NULL()
					& "CategoryName".AsColumn().IS_NULL()
					& Description.IS_NULL()
					)
				.ToString();

			Debug.Assert(SQL == "SELECT * FROM [Categories] WHERE ([CategoryId] IS NOT NULL AND [CategoryName] IS NULL) AND [Description] IS NULL");

		}

		[TestMethod]
		public void Test_IS_NOT_NULL2()
		{
			string sql = "SELECT COUNT(*) FROM [Products] WHERE [ProductId] IS NOT NULL";
			string query = new SqlBuilder().SELECT().COLUMNS(Expression.COUNT_STAR).FROM(Products).WHERE(ProductId != null).ToString();

			Debug.Assert(sql == query);
		}


		[TestMethod]
		public void Test_DATE()
		{
			var SQL = new SqlBuilder()
				.SELECT()
				.COLUMNS(
					"OrderDate".AsColumn().DATEDIFF(DateInterval.day, "ShippedDate".AsColumn()).AS("Day"),
					"Freight".AsColumn().CONVERT<int>().AS("Freight"),
					Expression.STAR)
				.FROM("Orders")
				.ToString();

			Debug.Assert(SQL == "SELECT DATEDIFF(day, [OrderDate], [ShippedDate]) AS Day, CONVERT(INT, [Freight]) AS Freight, * FROM [Orders]");

			var expr = "OrderDate".AsColumn().DATEADD(DateInterval.day, 5).ToString();
			Debug.Assert(expr == "DATEADD(day, 5, [OrderDate])");

			expr = "OrderDate".AsColumn().DATEPART(DateInterval.year).ToString();
			Debug.Assert(expr == "DATEPART(year, [OrderDate])");

			expr = "OrderDate".AsColumn().DATENAME(DateInterval.year).ToString();
			Debug.Assert(expr == "DATENAME(year, [OrderDate])");
		}



		[TestMethod]
		public void Test_TOP()
		{
			string sql = "SELECT TOP 20 * FROM [Products] WHERE [ProductId] < 10";
			string query = new SqlBuilder().SELECT().TOP(20).COLUMNS().FROM(Products).WHERE(ProductId < 10).ToString();

			Debug.Assert(sql == query);
		}

		[TestMethod]
		public void Test_IS_NULL()
		{
			string sql = "SELECT COUNT(*) FROM [Products] WHERE [ProductId] IS NULL";
			string query = new SqlBuilder().SELECT().COLUMNS(Expression.COUNT_STAR).FROM(Products).WHERE(ProductId.IS_NULL()).ToString();

			Debug.Assert(sql == query);
		}


		[TestMethod]
		public void Test_BETWEEN()
		{
			string sql = "SELECT COUNT(*), MAX([ProductId]) FROM [Products] WHERE [ProductId] BETWEEN 10 AND 30";
			string query = new SqlBuilder().SELECT().COLUMNS(Expression.COUNT_STAR, ProductId.MAX()).FROM(Products).WHERE(ProductId.BETWEEN(10, 30)).ToString();

			Debug.Assert(sql == query);

			sql = "SELECT COUNT(*), MAX([ProductId]) FROM [Products] WHERE [ProductId] NOT BETWEEN N'apple' AND N'pear'";
			query = new SqlBuilder().SELECT().COLUMNS(Expression.COUNT_STAR, ProductId.MAX()).FROM(Products).WHERE(ProductId.NOT_BETWEEN("apple", "pear")).ToString();

			Debug.Assert(sql == query);
		}




		[TestMethod]
		public void Test_CREATE_TABLE()
		{
			string sql = "CREATE TABLE [Purdue] ([Id] INT NOT NULL, [Time] DATETIME NOT NULL, [ENU] INT NOT NULL, [DATA] INT NOT NULL, PRIMARY KEY ([Id], [Time], [ENU], [DATA]))";

			var Id = "Id".AsColumn();
			var Time = "Time".AsColumn();
			var ENU = "ENU".AsColumn();
			var DATA = "DATA".AsColumn();

			string query = new SqlBuilder()
				.CREATE().TABLE("Purdue")
				.TUPLE(
		 			  Id.DEFINE_NOT_NULL(TYPE.INT),
					Time.DEFINE_NOT_NULL(TYPE.DATETIME),
					 ENU.DEFINE_NOT_NULL(TYPE.INT),
					DATA.DEFINE_NOT_NULL(TYPE.INT),
		 	  Expression.PRIMARY_KEY(Id, Time, ENU, DATA)
					)
				.ToString();

			Debug.Assert(sql == query);

			sql = "CREATE TABLE [Purdue] ([Id] INT NOT NULL, [Time] DATETIME NOT NULL, [ENU] INT NULL, [DATA] INT NULL, PRIMARY KEY ([Id], [Time]), FOREIGN KEY ([Id]) REFERENCES Products([ProductID]))";
			query = new SqlBuilder()
				.CREATE().TABLE("Purdue")
				.TUPLE(
					  Id.DEFINE_NOT_NULL(TYPE.INT),
					Time.DEFINE_NOT_NULL(TYPE.DATETIME),
					 ENU.DEFINE_NULL(TYPE.INT),
					DATA.DEFINE_NULL(TYPE.INT),
				Expression.PRIMARY_KEY(Id, Time),
						 Id.FOREIGN_KEY("Products", "ProductID".AsColumn())
					)
				.ToString();


			Debug.Assert(sql == query);
		}

		[TestMethod]
		public void Test_STORED_PROC()
		{
			string sql = @"CREATE PROCEDURE SelectAllCustomers @City NVARCHAR(30), @PostalCode NVARCHAR(10) AS
SELECT * FROM [Customers] WHERE ([City] = @City) AND ([PostalCode] = @PostalCode)
GO";
			string query = new Statement().CREATE().PROCEDURE("SelectAllCustomers" , "City".AsParameter(TYPE.NVARCHAR(30)), "PostalCode".AsParameter(TYPE.NVARCHAR(10))).AppendLine()
				.Append(new SqlBuilder()
				.SELECT().COLUMNS().FROM("Customers").WHERE("City".AsColumn() == "City".AsParameter() & "PostalCode".AsColumn() == "PostalCode".AsParameter()).AppendLine()
				.GO())
				.ToString();

			Debug.Assert(sql == query);

			sql = "EXECUTE SelectAllCustomers @City = N'London'";
			query = new Statement().EXECUTE("SelectAllCustomers").PARAMETERS("City".AsParameter().LET("London")).ToString();
			Debug.Assert(sql == query);

			query = new Statement().EXECUTE("SelectAllCustomers").PARAMETERS("City".AsParameter("London")).ToString();
			Debug.Assert(sql == query);

			query = new Statement().EXECUTE("SelectAllCustomers").PARAMETERS(new { City = "London" }).ToString();
			Debug.Assert(sql == query);

			query = new Statement().EXECUTE("SelectAllCustomers").PARAMETERS(new Dictionary<string, object> { ["City"] = "London" }).ToString();
			Debug.Assert(sql == query);
		}

		[TestMethod]
		public void Test_DECLARE()
		{
			string sql = "DECLARE @Age INT = 12, @City VARCHAR(50) = N'London'";
			string query = new Statement().DECLARE("@Age".AsVariable(TYPE.INT, 12), "@City".AsVariable(TYPE.VARCHAR(50), "London"))
				.ToString();

			Debug.Assert(sql == query);

			sql = "DECLARE Age INT = 12, City VARCHAR(50)";
			query = new Statement().DECLARE("Age".AsVariable(TYPE.INT, 12), "City".AsVariable(TYPE.VARCHAR(50), null))
				.ToString();

			Debug.Assert(sql == query);

			sql = "SET Age = 12, City = N'London'";
			query = new SqlBuilder().SET("Age".AsVariable(12), "City".AsVariable("London"))
				.ToString();

			Debug.Assert(sql == query);

		}

		[TestMethod]
		public void Test_CAST()
		{
			var OrderID = nameof(Orders.OrderID).AsColumn();
			var OrderDate = nameof(Orders.OrderDate).AsColumn();

			string sql = "SELECT CAST([OrderID] AS VARCHAR(10)) AS [Id], DATEADD(second, 100, CAST([OrderDate] AS DATETIME)) AS [Time] FROM [Orders]";
			string query = new SqlBuilder()
				.SELECT().COLUMNS
				(
					OrderID.CAST(TYPE.VARCHAR(10)).AS("[Id]"),
					OrderDate.CAST(TYPE.DATETIME).DATEADD(DateInterval.second, 100).AS("[Time]")
				)
				.FROM(nameof(Orders))
				.ToString();

			Debug.Assert(sql == query);

			sql = "SELECT [Id] = CAST([OrderID] AS VARCHAR(10)), [Time] = DATEADD(second, 100, CAST([OrderDate] AS DATETIME)) FROM [Orders]";
			query = new SqlBuilder()
				.SELECT().COLUMNS
				(
					"Id".AsColumn() == OrderID.CAST(TYPE.VARCHAR(10)),
					"Time".AsColumn() == OrderDate.CAST(TYPE.DATETIME).DATEADD(DateInterval.second, 100)
				)
				.FROM(nameof(Orders))
				.ToString();

			Debug.Assert(sql == query);
		}

		[TestMethod]
		public void Test_TRANSACTION()
		{
			string sql = @"BEGIN TRANSACTION
BEGIN TRY
	DELETE FROM [Products] WHERE [ProductID] = 980
END TRY
BEGIN CATCH
	SELECT ERROR_NUMBER() AS ErrorNumber, ERROR_SEVERITY() AS ErrorSeverity, ERROR_STATE() AS ErrorState, ERROR_PROCEDURE() AS ErrorProcedure, ERROR_LINE() AS ErrorLine, ERROR_MESSAGE() AS ErrorMessage
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
END CATCH

IF @@TRANCOUNT > 0 COMMIT TRANSACTION
GO
";

			var TRANCOUNT = "@@TRANCOUNT".AsVariable();

			var _try = new SqlBuilder().AppendTab().DELETE_FROM("Products").WHERE("ProductID".AsColumn() == 980);
			var _catch = new Statement()
				.AppendTab()
				.Append(new SqlBuilder().SELECT().COLUMNS(
						Expression.Function("ERROR_NUMBER").AS("ErrorNumber"),
						Expression.Function("ERROR_SEVERITY").AS("ErrorSeverity"),
						Expression.Function("ERROR_STATE").AS("ErrorState"),
						Expression.Function("ERROR_PROCEDURE").AS("ErrorProcedure"),
						Expression.Function("ERROR_LINE").AS("ErrorLine"),
						Expression.Function("ERROR_MESSAGE").AS("ErrorMessage")
					))
				.AppendLine()
				.AppendTab()
				.IF(TRANCOUNT > 0, new Statement().ROLLBACK_TRANSACTION());

			var statement = new Statement()
				.BEGIN_TRANSACTION()
				.TRY_CATCH(_try, _catch)
				.AppendLine()
				.IF(TRANCOUNT > 0, new Statement().COMMIT_TRANSACTION())
				.AppendLine()
				.GO()
				.ToString();


			Debug.Assert(sql == statement);

			statement = new Statement()
			.BEGIN_TRANSACTION()
			.TRY_CATCH(_try, _catch)
			.AppendLine()
			.IF(TRANCOUNT > 0).COMMIT_TRANSACTION()
			.GO()
			.ToString();


			Debug.Assert(sql == statement);
		}

		[TestMethod]
		public void Test_STORED_FUNCTION()
		{
			string sql = @"CREATE FUNCTION GetInventoryStock(@ProductID INT) RETURNS INT AS
-- Returns the stock level for the product.
BEGIN
	DECLARE @ret INT
	SELECT @ret = SUM([UnitsInStock]) FROM [Products] WHERE ([ProductID] = @ProductID) AND ([SupplierID] = 6)
	IF @ret IS NULL SET @ret = 0
	RETURN @ret
END";

			var ret = "ret".AsParameter();
			var prototype = new Statement()
				.CREATE().FUNCTION(TYPE.INT, "GetInventoryStock" ,"ProductID".AsParameter(TYPE.INT)).AppendLine()
				.COMMENTS(" Returns the stock level for the product.");
			
			var statement = new Statement()
				.Compound(
				new Statement().DECLARE("ret".AsParameter(TYPE.INT)),
				new SqlBuilder().SELECT().COLUMNS(ret== "UnitsInStock".AsColumn().SUM()).FROM("Products").WHERE("ProductID".AsColumn() == "ProductID".AsParameter() & "SupplierID".AsColumn() == 6),
				new Statement().IF(ret.IS_NULL(), new Statement().LET(ret, 0)),
				new Statement().RETURN(ret)
				);

			var query = new Statement()
				.Append(prototype)
				.Append(statement)
				.ToString();

			Debug.Assert(sql == query);

			sql = "EXECUTE @ret = GetInventoryStock @ProductID = 20";
			query = new Statement()
				.EXECUTE(ret, "GetInventoryStock")
				.PARAMETERS("ProductID".AsParameter().LET(20))
				.ToString();

			Debug.Assert(sql == query);
		}
	}
}


