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
using System.Data.Common;
using Sys.Data.Entity;
using Sys.Data.Text;

namespace Sys.Data
{
	/// <summary>
	/// Agent to access database engine/server
	/// </summary>
	public abstract class DbAgent : IDbAgent
	{
		protected readonly DbConnectionStringBuilder ConnectionString;

		protected DbAgent(DbConnectionStringBuilder connectionString)
		{
			this.ConnectionString = connectionString;
		}

		public abstract DbAgentOption Option { get; }

		public abstract IDbAccess Access(SqlUnit unit);

		public override string ToString()
		{
			return ConnectionString.ToString();
		}
	}
}