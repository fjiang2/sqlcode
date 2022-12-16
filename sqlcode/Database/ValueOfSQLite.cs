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
using System.Text;

namespace Sys.Data
{
	class ValueOfSQLite : ValueOfScript
	{
		public ValueOfSQLite(object value)
			: base(value)
		{

		}

		protected override string ToScript(string value)
		{
			return new StringBuilder()
			.Append(DELIMETER)
			.Append(value.Replace("'", "''"))
			.Append(DELIMETER)
			.ToString();
		}

		protected override string ToScript(byte[] data)
		{
			return new StringBuilder()
			.Append("x")
			.Append(DELIMETER)
			.Append(BitConverter.ToString(data).Replace("-", ""))
			.Append(DELIMETER)
			.ToString();
		}

		protected override string ToScript(Guid value)
		{
			return new StringBuilder()
			.Append(DELIMETER)
			.Append(value)
			.Append(DELIMETER)
			.ToString();
		}
	}
}
