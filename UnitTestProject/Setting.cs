using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject
{
	class Setting
	{
		public static string ConnectionString
		{
			get
			{
				if (Environment.MachineName.StartsWith("XPS"))
					return "data source=localhost\\sqlexpress;initial catalog=Northwind;integrated security=SSPI;packet size=4096";
				else
					return "Server = (LocalDB)\\MSSQLLocalDB;initial catalog=Northwind;Integrated Security = true;";
			}
		}
	}
}
