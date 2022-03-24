using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

using System.Data.SQLite;
using Sys.Data.Entity;

namespace Sys.Data.SQLite
{
	public class DbContext : DataContext
	{
		public DbContext(string connectionString)
			: this(new SQLiteConnectionStringBuilder(connectionString))
		{
		}

		public DbContext(SQLiteConnectionStringBuilder connectionString)
			: base(new SQLiteAgent(connectionString))
		{
		}
	}
}
