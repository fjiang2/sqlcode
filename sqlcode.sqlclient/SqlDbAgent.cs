﻿#if NET48
using System.Data.SqlClient;
#else
using Microsoft.Data.SqlClient;
#endif

using Sys.Data.Entity;

namespace Sys.Data.SqlClient
{
	public class SqlDbAgent : DbAgent
	{
		public SqlDbAgent(SqlConnectionStringBuilder connectionString)
			: base(connectionString)
		{
		}

		public override DbAgentOption Option => new DbAgentOption { Style = DbAgentStyle.SqlServer };
		public override IDbAccess Access(SqlUnit unit) => new SqlDbAccess(ConnectionString.ConnectionString, unit);

	}
}
