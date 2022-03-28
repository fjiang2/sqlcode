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
			: this(new SqlCeConnectionStringBuilder(connectionString))
		{
		}

		public DbContext(SqlCeConnectionStringBuilder connectionString)
			: this(new SqlCeAgent(connectionString))
		{
		}

		public DbContext(SqlCeAgent agent)
			: base(agent)
		{
		}

	}
}
