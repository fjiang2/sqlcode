﻿//--------------------------------------------------------------------------------------------------//
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
using System.Linq;
using System.Text;

namespace Sys.Data
{
	/// <summary>
	/// a value can be used on SQL statement
	/// </summary>
	public class SqlValue
	{
		private const string DELIMETER = "'";
		private const string NULL = "NULL";

		private readonly object value;

		public SqlValue(object value)
		{
			if (value is SqlValue v)
				this.value = v.value;

			this.value = value;
		}

		public object Value => value;

		public bool IsNull => value == null || value == DBNull.Value;

		public string ToScript(DbAgentStyle style)
		{
			if (IsNull)
				return NULL;

			StringBuilder sb = new StringBuilder();

			if (value is string)
			{
				//N: used for SQL Type nvarchar
				if (style != DbAgentStyle.SQLite)
					sb.Append("N");

				sb.Append(DELIMETER)
				.Append((value as string).Replace("'", "''"))
				.Append(DELIMETER);
			}
			else if (value is bool || value is bool?)
			{
				sb.Append((bool)value ? "1" : "0");
			}
			else if (value is DateTime || value is DateTime?)
			{
				DateTime time = (DateTime)value;
				sb.Append(DELIMETER)
				  .AppendFormat("{0} {1}", time.ToString("d"), time.ToString("HH:mm:ss.fff"))
				  .Append(DELIMETER);
			}
			else if (value is DateTimeOffset || value is DateTimeOffset?)
			{
				DateTimeOffset time = (DateTimeOffset)value;
				var d = DELIMETER + string.Format("{0} {1}", time.ToString("d"), time.ToString("HH:mm:ss.fff zzz"), time.Offset) + DELIMETER;
				return d;
			}
			else if (value is char)
			{
				sb.Append(DELIMETER).Append(value).Append(DELIMETER);
			}
			else if (value is byte[])
			{
				if (style != DbAgentStyle.SQLite)
					sb.Append("0x").Append(BitConverter.ToString((byte[])value).Replace("-", ""));
				else
					sb.Append("x").Append(DELIMETER).Append(BitConverter.ToString((byte[])value).Replace("-", "")).Append(DELIMETER);
			}
			else if (value is Guid || value is Guid?)
			{
				if (style != DbAgentStyle.SQLite)
					sb.Append("N" + DELIMETER).Append(value).Append(DELIMETER);
				else
					sb.Append(DELIMETER).Append(value).Append(DELIMETER);
			}
			else if (value is IEnumerable)
			{
				List<string> list = new List<string>();
				foreach (var x in value as IEnumerable)
				{
					list.Add(new SqlValue(x).ToScript(style));
				}
				return $"({string.Join(",", list)})";
			}
			else
			{
				sb.Append(value);
			}

			return sb.ToString();
		}

		public override string ToString()
		{
			return this.ToScript(DbAgentOption.DefaultStyle);
		}

	}
}
