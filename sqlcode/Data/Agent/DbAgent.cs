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
using Sys.Data.Entity;

namespace Sys.Data
{
	public abstract class DbAgent : IDbAgent
	{
		protected DbAgent()
		{
		}

		public abstract DbAgentOption Option { get; }

		public abstract IDbAccess Proxy(SqlUnit unit);

		public DataContext DataContext() => new DataContext(this);

		public DataQuery Query() => new DataQuery(this);

		public DbAccess Query(string sql, object args = null)
		{
			var unit = new SqlUnit(sql, args);
			
			var proxy = Proxy(unit);
			if (proxy is DbAccess access)
				return access;
			else
				return new DbAccessDelegate(this, unit);
		}

		public override string ToString()
		{
			return Option.ToString();
		}
	}
}