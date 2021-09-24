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

namespace Sys.Data
{

	/// <summary>
	///  Database agent option
	/// </summary>
	public class DbAgentOption
	{
		/// <summary>
		/// The default database agent option
		/// </summary>
		public readonly static DbAgentOption DefaultOption = new DbAgentOption() { Style = DefaultStyle };

		/// <summary>
		/// The default database agent style
		/// </summary>
		public readonly static DbAgentStyle DefaultStyle = DbAgentStyle.SqlServer;

		/// <summary>
		/// 
		/// </summary>
		public DbAgentStyle Style { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DbAgentOption()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Style}";
		}
	}
}
