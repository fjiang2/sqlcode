﻿using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
	public class SqlAgent : DbAgent
	{
		private SqlConnectionStringBuilder connectionString;

		public SqlAgent(SqlConnectionStringBuilder connectionString)
		{
			this.connectionString = connectionString;
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public override IDbAccess Proxy(SqlUnit unit) => new SqlAccess(connectionString, unit);

		public static DataQuery Query(string connectionString) 
			=> new DataQuery(new SqlAgent(new SqlConnectionStringBuilder(connectionString)));
	}
}
