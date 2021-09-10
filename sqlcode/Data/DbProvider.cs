//--------------------------------------------------------------------------------------------------//
//                                                                                                  //
//        DPO(Data Persistent Object)                                                               //
//                                                                                                  //
//          Copyright(c) Datum Connect Inc.                                                         //
//                                                                                                  //
// This source code is subject to terms and conditions of the Datum Connect Software License. A     //
// copy of the license can be found in the License.html file at the root of this distribution. If   //
// you cannot locate the  Datum Connect Software License, please send an email to                   //
// datconn@gmail.com. By using this source code in any fashion, you are agreeing to be bound        //
// by the terms of the Datum Connect Software License.                                              //
//                                                                                                  //
// You must not remove this notice, or any other, from this software.                               //
//                                                                                                  //
//                                                                                                  //
//--------------------------------------------------------------------------------------------------//

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;

namespace Sys.Data
{
	public abstract class DbProvider : IDbCmd
	{
		public DbConnection Connection { get; }
		public DbCommand Command { get; }

		public DbProvider(DbConnectionStringBuilder connectionString, string script, object parameters)
		{
			this.Connection = NewDbConnection(connectionString);
			this.Command = NewDbCommand(script, parameters);
		}

		protected abstract DbConnection NewDbConnection(DbConnectionStringBuilder connectionString);
		protected abstract DbCommand NewDbCommand(string script, object parameters);
		protected abstract DbDataAdapter NewDbDataAdapter(DbCommand command);

		public DataSet FillDataSet(DataSet ds)
		{
			try
			{
				Connection.Open();
				DbDataAdapter adapter = NewDbDataAdapter(Command);
				adapter.Fill(ds);
				return ds;
			}
			finally
			{
				Connection.Close();
			}
		}

		public int FillTable(DataTable dt, int startRecord, int maxRecords)
		{
			try
			{
				Connection.Open();
				DbDataAdapter adapter = NewDbDataAdapter(Command);
				return adapter.Fill(startRecord, maxRecords, dt);
			}
			finally
			{
				Connection.Close();
			}
		}

		public int ExecuteNonQuery()
		{
			try
			{
				Connection.Open();
				int n = Command.ExecuteNonQuery();
				return n;
			}
			finally
			{
				Connection.Close();
			}
		}


		public object ExecuteScalar()
		{
			try
			{
				Connection.Open();
				return Command.ExecuteScalar();
			}
			finally
			{
				Connection.Close();
			}
		}

	}
}

