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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sys.Data
{
	public class ValueOfScript
	{
		protected const string DELIMETER = "'";
		protected const string NULL = "NULL";
		private readonly object value;

		public ValueOfScript(object value)
		{
			this.value = value;
		}

		protected virtual string ToScript(string value)
		{
			return new StringBuilder()
			.Append("N")
			.Append(DELIMETER)
			.Append(value.Replace("'", "''"))
			.Append(DELIMETER)
			.ToString();
		}

		protected virtual string ToScript(bool value)
		{
			return new StringBuilder()
			.Append(value ? "1" : "0")
			.ToString();
		}

		protected virtual string ToScript(char value)
		{
			return new StringBuilder()
			.Append(DELIMETER)
			.Append(value)
			.Append(DELIMETER)
			.ToString();
		}

		protected virtual string ToScript(DateTime time)
		{
			return new StringBuilder()
			.Append(DELIMETER)
			.AppendFormat("{0} {1}", time.ToString("d"), time.ToString("HH:mm:ss.fff"))
			.Append(DELIMETER)
			.ToString();
		}

		protected virtual string ToScript(DateTimeOffset time)
		{
			return new StringBuilder()
			.Append(DELIMETER)
			.AppendFormat("{0} {1}", time.ToString("d"), time.ToString("HH:mm:ss.fff zzz"), time.Offset)
			.Append(DELIMETER)
			.ToString();
		}

		protected virtual string ToScript(byte[] data)
		{
			return new StringBuilder()
			.Append("0x")
			.Append(BitConverter.ToString(data).Replace("-", ""))
			.ToString();
		}

		protected virtual string ToScript(Guid value)
		{
			return new StringBuilder()
			.Append("N")
			.Append(DELIMETER)
			.Append(value)
			.Append(DELIMETER)
			.ToString();
		}

		protected virtual string ToScript(IEnumerable value)
		{
			List<string> list = new List<string>();
			foreach (var x in value)
			{
				list.Add(new ValueOfScript(x).ToScript());
			}
			return $"({string.Join(",", list)})";
		}

		public virtual string ToScript()
		{
			if (value == null || value == DBNull.Value)
				return NULL;

			if (value is string str)
				return ToScript(str);

			if (value is bool || value is bool?)
				return ToScript((bool)value);

			if (value is DateTime || value is DateTime?)
				return ToScript((DateTime)value);

			if (value is DateTimeOffset || value is DateTimeOffset?)
				return ToScript((DateTimeOffset)value);

			if (value is char ch)
				return ToScript(ch);

			if (value is byte[] data)
				return ToScript(data);

			if (value is Guid || value is Guid?)
				return ToScript((Guid)value);

			if (value is IEnumerable list)
				return ToScript(list);

			return new StringBuilder().Append(value).ToString();
		}

		public override string ToString()
		{
			return ToScript();
		}
	}
}
