﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Sys.Data
{
	class DelegateDbCmd : BaseDbCmd, IDbCmd
	{
		private IDbProvider provider;
		private IDbCmd command;

		public DelegateDbCmd(IDbProvider provider, string sql, object args)
		{
			this.Description = "delegate DbCmd";
			this.provider = provider;
			this.command = provider.Function(sql, args);
		}

		public DbProviderOption Option => provider.Option;
		public override int ExecuteNonQuery() => command.ExecuteNonQuery();
		public override object ExecuteScalar() => command.ExecuteScalar();
		public override int FillDataSet(DataSet dataSet) => command.FillDataSet(dataSet);
		public override int FillDataTable(DataTable dataTable, int startRecord, int maxRecords) => command.FillDataTable(dataTable, startRecord, maxRecords);
	}
}
