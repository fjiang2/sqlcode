using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Sys.Data
{
	public class DelegateDbCmd : BaseDbCmd
	{
		private DbCmdFunc dbCommand;
		private string sql;
		private object args;

		public DelegateDbCmd(DbCmdFunc cmd, string sql, object args)
		{
			this.Description = "delegate DbCmd";
			this.dbCommand = cmd;
			this.sql = sql;
			this.args = args;
		}

		public override int ExecuteNonQuery()
		{
			return dbCommand(sql, args).ExecuteNonQuery();
		}

		public override object ExecuteScalar()
		{
			return dbCommand(sql, args).ExecuteScalar();
		}

		public override DataSet FillDataSet(DataSet dataSet)
		{
			return dbCommand(sql, args).FillDataSet(dataSet);
		}
	}
}
