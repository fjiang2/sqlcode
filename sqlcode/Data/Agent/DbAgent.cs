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
	/// <summary>
	/// Agent to access database engine/server
	/// </summary>
	public abstract class DbAgent : IDbAgent
	{
		protected DbAgent()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract DbAgentOption Option { get; }

		/// <summary>
		/// proxy connects to database
		/// </summary>
		/// <param name="unit">Sql code unit</param>
		/// <returns></returns>
		public abstract IDbAccess Proxy(SqlUnit unit);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public abstract DbAccess Unit(string query, object args);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public DataContext Context() => new DataContext(this);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public DataQuery Query() => new DataQuery(this);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Option.ToString();
		}
	}
}