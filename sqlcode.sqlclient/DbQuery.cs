﻿#if NET48
using System.Data.SqlClient;
#else
using Microsoft.Data.SqlClient;
#endif
using Sys.Data.Entity;



namespace Sys.Data.SqlClient
{
	public class DbQuery : DataQuery
	{
		public DbQuery(string connectionString)
			: this(new SqlConnectionStringBuilder(connectionString))
		{
		}

		public DbQuery(SqlConnectionStringBuilder connectionString)
			: base(new SqlDbAgent(connectionString))
		{
		}

		public DbQuery(SqlDbAgent agent)
			: base(agent)
		{
		}
	}
}
