using System;
using System.Data.Common;
using Sys.Data.Entity;

namespace Sys.Data.SqlRemote
{
	public class DbQuery : DataQuery
	{
	
		public DbQuery(ISqlMessageClient client)
			: base(new SqlRemoteAgent(client))
		{
		}

		public DbQuery(SqlRemoteAgent agent)
			: base(agent)
		{
		}
	}
}
