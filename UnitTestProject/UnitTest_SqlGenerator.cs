using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sys.Data;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest_SqlGenerator
	{
		private SqlGenerator gen;

		public const string _CATEGORYID = "CategoryID";
		public const string _CATEGORYNAME = "CategoryName";
		public const string _DESCRIPTION = "Description";
		public const string _PICTURE = "Picture";

		public UnitTest_SqlGenerator()
		{
			gen = new SqlGenerator("[Categories]")
			{
				PrimaryKeys = new string[] { _CATEGORYID },
				IdentityKeys = new string[] { _CATEGORYID },
			};
		}

		[TestMethod]
		public void Test_SELECT()
		{
			gen.Option = new SqlOption { Style = SqlStyle.SqlServer};
			gen.Clear();
			gen.Add(_CATEGORYID, 1);

			string SQL = gen.Select();
			Debug.Assert(SQL == "SELECT * FROM [Categories] WHERE [CategoryID] = 1");

			SQL = gen.SelectRows();
			Debug.Assert(SQL == "SELECT * FROM [Categories]");

			SQL = gen.SelectRows(new string[] { _CATEGORYNAME, _DESCRIPTION });
			Debug.Assert(SQL == "SELECT [CategoryName],[Description] FROM [Categories]");

		}


		[TestMethod]
		public void Test_INSERT()
		{
			gen.Option = new SqlOption { Style = SqlStyle.SqlServer };
			gen.Clear();
			gen.Add(_CATEGORYID, 12);
			gen.Add(_CATEGORYNAME, "Drink");
			gen.Add(_DESCRIPTION, "Water");
			gen.Add(_PICTURE, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

			string SQL = gen.Insert();
			Debug.Assert(SQL == "INSERT INTO [Categories]([CategoryName],[Description],[Picture]) VALUES(N'Drink',N'Water',0x0102030405060708)");

			SQL = gen.InsertWithIdentityParameter();
			Debug.Assert(SQL == "INSERT INTO [Categories]([CategoryName],[Description],[Picture]) VALUES(N'Drink',N'Water',0x0102030405060708); SET @CategoryID=@@IDENTITY");

			SQL = gen.InsertOrUpdate();
			Debug.Assert(SQL == "IF EXISTS(SELECT * FROM [Categories] WHERE [CategoryID] = 12) UPDATE [Categories] SET [CategoryName] = N'Drink',[Description] = N'Water',[Picture] = 0x0102030405060708 WHERE [CategoryID] = 12 ELSE INSERT INTO [Categories]([CategoryName],[Description],[Picture]) VALUES(N'Drink',N'Water',0x0102030405060708)");

			SQL = gen.InsertIfNotExists();
			Debug.Assert(SQL == "IF NOT EXISTS(SELECT * FROM [Categories] WHERE [CategoryID] = 12) INSERT INTO [Categories]([CategoryName],[Description],[Picture]) VALUES(N'Drink',N'Water',0x0102030405060708)");

			SQL = gen.InsertOrUpdate(exists: true);
			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = N'Drink',[Description] = N'Water',[Picture] = 0x0102030405060708 WHERE [CategoryID] = 12");

			SQL = gen.InsertOrUpdate(exists: false);
			Debug.Assert(SQL == "INSERT INTO [Categories]([CategoryName],[Description],[Picture]) VALUES(N'Drink',N'Water',0x0102030405060708)");
		}

		[TestMethod]
		public void Test_SQLite_INSERT()
		{
			gen.Option = new SqlOption { Style = SqlStyle.SQLite };
			gen.Clear();
			gen.Add(_CATEGORYID, 12);
			gen.Add(_CATEGORYNAME, "Drink");
			gen.Add(_DESCRIPTION, "Water");
			gen.Add(_PICTURE, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

			string SQL = gen.Insert();
			Debug.Assert(SQL == "INSERT INTO [Categories]([CategoryName],[Description],[Picture]) VALUES('Drink','Water',x'0102030405060708')");

			SQL = gen.InsertWithIdentityParameter();
			Debug.Assert(SQL == "INSERT INTO [Categories]([CategoryName],[Description],[Picture]) VALUES('Drink','Water',x'0102030405060708'); SET @CategoryID=@@IDENTITY");

			SQL = gen.InsertOrUpdate();
			Debug.Assert(SQL == "IF EXISTS(SELECT * FROM [Categories] WHERE [CategoryID] = 12) UPDATE [Categories] SET [CategoryName] = 'Drink',[Description] = 'Water',[Picture] = x'0102030405060708' WHERE [CategoryID] = 12 ELSE INSERT INTO [Categories]([CategoryName],[Description],[Picture]) VALUES('Drink','Water',x'0102030405060708')");

			SQL = gen.InsertIfNotExists();
			Debug.Assert(SQL == "IF NOT EXISTS(SELECT * FROM [Categories] WHERE [CategoryID] = 12) INSERT INTO [Categories]([CategoryName],[Description],[Picture]) VALUES('Drink','Water',x'0102030405060708')");

			SQL = gen.InsertOrUpdate(exists: true);
			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = 'Drink',[Description] = 'Water',[Picture] = x'0102030405060708' WHERE [CategoryID] = 12");

			SQL = gen.InsertOrUpdate(exists: false);
			Debug.Assert(SQL == "INSERT INTO [Categories]([CategoryName],[Description],[Picture]) VALUES('Drink','Water',x'0102030405060708')");
		}

		[TestMethod]
		public void Test_SQLite_UPDATE()
		{
			gen.Option = new SqlOption { Style = SqlStyle.SQLite };
			gen.Clear();
			gen.Add(_CATEGORYID, 12);
			gen.Add(_CATEGORYNAME, "Drink");
			gen.Add(_DESCRIPTION, "Water");
			gen.Add(_PICTURE, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

			string SQL = gen.Update();
			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = 'Drink',[Description] = 'Water',[Picture] = x'0102030405060708' WHERE [CategoryID] = 12");

			SQL = gen.UpdateIfExists();
			Debug.Assert(SQL == "IF EXISTS(SELECT * FROM [Categories] WHERE [CategoryID] = 12) UPDATE [Categories] SET [CategoryName] = 'Drink',[Description] = 'Water',[Picture] = x'0102030405060708' WHERE [CategoryID] = 12");

			gen.Remove(_PICTURE);
			SQL = gen.Update();
			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = 'Drink',[Description] = 'Water' WHERE [CategoryID] = 12");

			gen.Clear();
			gen.Add(_CATEGORYID, 12);
			SQL = gen.Update();
			Debug.Assert(SQL == "");
		}

		[TestMethod]
		public void Test_UPDATE()
		{
			gen.Option = new SqlOption { Style = SqlStyle.SqlServer };
			gen.Clear();
			gen.Add(_CATEGORYID, 12);
			gen.Add(_CATEGORYNAME, "Drink");
			gen.Add(_DESCRIPTION, "Water");
			gen.Add(_PICTURE, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

			string SQL = gen.Update();
			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = N'Drink',[Description] = N'Water',[Picture] = 0x0102030405060708 WHERE [CategoryID] = 12");

			SQL = gen.UpdateIfExists();
			Debug.Assert(SQL == "IF EXISTS(SELECT * FROM [Categories] WHERE [CategoryID] = 12) UPDATE [Categories] SET [CategoryName] = N'Drink',[Description] = N'Water',[Picture] = 0x0102030405060708 WHERE [CategoryID] = 12");

			gen.Remove(_PICTURE);
			SQL = gen.Update();
			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = N'Drink',[Description] = N'Water' WHERE [CategoryID] = 12");

			gen.Clear();
			gen.Add(_CATEGORYID, 12);
			SQL = gen.Update();
			Debug.Assert(SQL == "");
		}


		[TestMethod]
		public void Test_DELETE()
		{
			gen.Option = new SqlOption { Style = SqlStyle.SqlServer };
			gen.Clear();
			gen.Add(_CATEGORYID, 12);

			string SQL = gen.Delete();
			Debug.Assert(SQL == "DELETE FROM [Categories] WHERE [CategoryID] = 12");

			SQL = gen.DeleteAll();
			Debug.Assert(SQL == "DELETE FROM [Categories]");
		}

		[TestMethod]
		public void Test_AddRange()
		{
			gen.Option = new SqlOption { Style = SqlStyle.SqlServer };
			gen.Clear();
			gen.AddRange(new
			{
				CategoryID = 12,
				CategoryName = "Drink",
				Description = "Water"
			});
			string SQL = gen.Update();
			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = N'Drink',[Description] = N'Water' WHERE [CategoryID] = 12");

			gen.Clear();
			gen.AddRange(new Dictionary<string, object>
			{
				[_CATEGORYID] = 12,
				[_CATEGORYNAME] = "Drink",
				[_DESCRIPTION] = "Water"
			});

			SQL = gen.Update();
			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = N'Drink',[Description] = N'Water' WHERE [CategoryID] = 12");


			gen.Clear();
			gen.AddRange(new string[] { _CATEGORYID, _CATEGORYNAME, _DESCRIPTION }, new object[] { 12, "Drink", "Water" });
			SQL = gen.Update();
			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = N'Drink',[Description] = N'Water' WHERE [CategoryID] = 12");

			gen.RemoveRange(new string[] { _DESCRIPTION, _PICTURE });
			SQL = gen.Update();
			Debug.Assert(SQL == "UPDATE [Categories] SET [CategoryName] = N'Drink' WHERE [CategoryID] = 12");

			gen.Clear();
			gen.AddRange<int>("C", new int[] { 12, 13, 14 });
			SQL = gen.Update();
			Debug.Assert(SQL == "UPDATE [Categories] SET [C1] = 12,[C2] = 13,[C3] = 14");

			gen.Clear();
			gen.Add(_CATEGORYID, 12);
			gen.AddRange("C", new object[] { 12, "Drink", "Water" });
			SQL = gen.Update();
			Debug.Assert(SQL == "UPDATE [Categories] SET [C1] = 12,[C2] = N'Drink',[C3] = N'Water' WHERE [CategoryID] = 12");
		}
	}
}

