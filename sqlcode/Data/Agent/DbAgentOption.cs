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
	public class DbAgentOption
	{
		public readonly static DbAgentOption DefaultOption = new DbAgentOption() { Style = DefaultStyle };
		public readonly static DbAgentStyle DefaultStyle = DbAgentStyle.SqlServer;

		public DbAgentStyle Style { get; set; }

		public DbAgentOption()
		{
		}

		public override string ToString()
		{
			return $"{Style}";
		}
	}
}
