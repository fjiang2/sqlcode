using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Sys.Data
{
	class DbAccessDelegate : DbAccess, IDbAccess
	{
		private readonly IDbAccess command;

		public DbAccessDelegate(IDbAccess access)
		{
			this.Description = nameof(DbAccessDelegate);
			this.command = access;
		}

		public override int ExecuteNonQuery() => command.ExecuteNonQuery();
		public override object ExecuteScalar() => command.ExecuteScalar();
		public override int FillDataSet(DataSet dataSet) => command.FillDataSet(dataSet);
		public override int FillDataTable(DataTable dataTable, int startRecord, int maxRecords) => command.FillDataTable(dataTable, startRecord, maxRecords);
		public override int ReadDataSet(DataSet dataSet) => command.ReadDataSet(dataSet);
		public override int ReadDataTable(DataTable dataTable, int startRecord, int maxRecords) => command.ReadDataTable(dataTable, startRecord, maxRecords);
		public override void ExecuteTransaction() => command.ExecuteTransaction();
	}
}
