using System;
using System.Collections.Generic;
using System.Data;

namespace Sys.Data
{
	static class Extension
	{
		public static T IsNull<T>(this object value, T defaultValue = default(T))
		{
			if (value == null || value == DBNull.Value)
				return defaultValue;

			if (value is not T)
				value = Convert.ChangeType(value, typeof(T));

			if (value is T)
				return (T)value;

			throw new Exception($"{value} is not type of {typeof(T)}");
		}

		public static T GetField<T>(this DataRow row, string columnName, T defaultValue = default(T))
		{
			if (!row.Table.Columns.Contains(columnName))
				return defaultValue;

			return IsNull<T>(row[columnName], defaultValue);
		}
	}
}
