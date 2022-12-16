using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sys.Data
{
	static class Extension
	{
		public static T IsNull<T>(this object value, T defaultValue = default(T))
		{
			if (value == null || value == DBNull.Value)
				return defaultValue;

			//SQLite treats byte,short,int,and long to be long
			if (!(value is T))
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

        public static void SetField<T>(this DataRow row, string columnName, T value)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                row[columnName] = value;
            }
            else
            {
                throw new Exception($"Cannot find column name {columnName}");
            }
        }

        public static IEnumerable<DataRow> AsEnumerable(this DataTable table)
        {
            return table.Rows.OfType<DataRow>();
        }
        
    }
}
