using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Sys.Data
{
	class DelegateDbCmd : BaseDbCmd, IDbCmd
	{
		private readonly IDbAgent agent;
		private readonly IDbCmd command;

		public DelegateDbCmd(IDbAgent agent, SqlUnit unit)
		{
			this.Description = "delegate DbCmd";
			this.agent = agent;
			this.command = agent.Proxy(unit);
		}

		public IDbAgent Provider => agent;
		public override int ExecuteNonQuery() => command.ExecuteNonQuery();
		public override object ExecuteScalar() => command.ExecuteScalar();
		public override int FillDataSet(DataSet dataSet) => command.FillDataSet(dataSet);
		public override int FillDataTable(DataTable dataTable, int startRecord, int maxRecords) => command.FillDataTable(dataTable, startRecord, maxRecords);
		public override void BulkInsert(DataTable dataTable, int batchSize) => command.BulkInsert(dataTable, batchSize);
	}
}
