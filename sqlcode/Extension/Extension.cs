using System;
using System.Collections.Generic;
using System.Data;

namespace Sys.Data
{
	static class Extension
	{
		public static T IsNull<T>(this object value)
		{
			if (value == null || value == DBNull.Value)
				return default(T);

			if (value is T)
				return (T)value;

			throw new Exception($"{value} is not type of {typeof(T)}");
		}

		//public static string ToScript(this SqlValue value, DbAgentStyle style)
		//{
		//	if (style == DbAgentStyle.SQLite)
		//		return new SQLiteValue(value.Value).ToScript();
		//	else
		//		return value.ToScript();
		//}
	}
}
