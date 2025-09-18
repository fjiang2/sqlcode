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
					return "data source=localhost\\SQLEXPRESS;initial catalog=Northwind;integrated security=SSPI;packet size=4096";
                else if (Environment.MachineName.StartsWith("LENOVO"))
                    return "data source=localhost\\SQLEXPRESS,20283;initial catalog=Northwind;User Id=sa;Password=JPO-1024$";
                else
                    //return "Server = (LocalDB)\\MSSQLLocalDB;initial catalog=Northwind;Integrated Security = true;";
                    return "Server = localhost\\SQLEXPRESS;initial catalog=Northwind;Integrated Security = true;";
            }
        }
	}
}
