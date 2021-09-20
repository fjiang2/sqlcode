using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using System.Data.SqlServerCe;
using Sys.Data.Entity;

namespace Sys.Data.SqlCe
{
	public class DbContext : DataContext
	{
		public DbContext(string connectionString)
			: base(new SqlCeAgent(new SqlCeConnectionStringBuilder(connectionString)))
		{
		}
	}
}
