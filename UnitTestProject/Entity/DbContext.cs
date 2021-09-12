using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Sys.Data.Entity;
using Sys.Data;

namespace UnitTestProject
{
	public class DbContext : DataContext
	{
		public DbContext(string connectionString)
			: base(DbAgent.Create(DbAgentStyle.SqlServer, (query, args) => new SqlCmd(new SqlConnectionStringBuilder(connectionString), query, args)))
		{
		}
	}
}
