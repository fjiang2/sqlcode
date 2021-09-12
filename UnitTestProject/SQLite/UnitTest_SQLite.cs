using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.IO;

using UnitTestProject.Northwind.dc1;
using Sys;
using Sys.Data.Entity;
using Sys.Data;

namespace UnitTestProject
{
	/// <summary>
	/// Summary description for UnitTestDataContext
	/// </summary>
	[TestClass]
	public class UnitTest_SQLite
	{
		private readonly Query Query;

		public UnitTest_SQLite()
		{
			DataContext.EntityClassType = EntityClassType.ExtensionClass;
			Query = new Query(new SQLiteAgent("..\\..\\..\\db\\Northwind.db"));
		}

		//[TestMethod]
		public void InsertAllRows()
		{
			string[] lines = File.ReadAllLines("..\\..\\..\\db\\sqlite-northwind-insert.sql");
			foreach (string line in lines)
			{
				if (line == "GO")
					continue;

				Query.NewDbCmd(line, null).ExecuteNonQuery();
			}
		}

		[TestMethod]
		public void UpdateRows()
		{
		}
	}
}
