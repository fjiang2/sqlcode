using System;
using System.Data.Common;
using Sys.Data.Entity;

namespace Sys.Data.SqlRemote
{
	public class DbQuery : DataQuery
	{
	
		public DbQuery(ISqlRemoteBroker broker)
			: base(new SqlRemoteAgent(broker))
		{
		}

		public DbQuery(SqlRemoteAgent agent)
			: base(agent)
		{
		}
	}
}
