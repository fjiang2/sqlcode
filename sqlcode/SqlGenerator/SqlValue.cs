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
using System.Linq;

namespace Sys.Data
{
	/// <summary>
	/// a value can be used on SQL statement
	/// </summary>
	public class SqlValue
	{
		private readonly object value;

		public SqlValue(object value)
		{
			if (value is SqlValue v)
				this.value = v.value;

			this.value = value;
		}

		public object Value => value;

		public bool IsNull => value == null || value == DBNull.Value;

        public override bool Equals(object obj)
        {
			SqlValue sqlValue = obj as SqlValue;
			
			if (sqlValue == null) 
				return false;

			if (this.IsNull && sqlValue.IsNull)
				return true;

			return this.value.Equals(sqlValue.value);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public string ToScript(DbAgentStyle style)
		{
			return Facade.ToScript(this, style);
		}

		public string ToString(DbAgentStyle style)
		{
			return ToScript(style);
		}

		public override string ToString()
		{
			return ToScript(DbAgentOption.DefaultStyle);
		}
	}
}
