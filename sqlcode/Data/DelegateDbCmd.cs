using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Sys.Data
{
	public class DelegateDbCmd : BaseDbCmd
	{
		private Func<string, IDbCmd> dbCommand;
		private string sql;

		public DelegateDbCmd(Func<string, IDbCmd> cmd, string sql)
		{
			this.Description = "delegate DbCmd";
			this.dbCommand = cmd;
			this.sql = sql;
		}

		public override int ExecuteNonQuery()
		{
			return dbCommand(sql).ExecuteNonQuery();
		}

		public override object ExecuteScalar()
		{
			return dbCommand(sql).ExecuteNonQuery();
		}

		public override DataSet FillDataSet(DataSet dataSet)
		{
			return dbCommand(sql).FillDataSet(dataSet);
		}
	}
}
