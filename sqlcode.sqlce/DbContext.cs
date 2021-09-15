using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Sys.Data.Entity;
using Sys.Data;

namespace Sys.Data.SqlCe
{
	public class DbContext : DataContext
	{
		public DbContext(string connectionString)
			: base(new SqlCeAgent(connectionString))
		{
		}
	}
}
