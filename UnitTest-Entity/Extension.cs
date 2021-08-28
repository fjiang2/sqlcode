using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace UnitTestProject
{
	public static class Extension
	{
		public static T GetField<T>(this DataRow row, string columnName)
		{
			object value= row[columnName];

			if (value is T)
				return (T)value;

			if (value == null || value == DBNull.Value)
				return default(T);

			throw new Exception($"{value} is not type of {typeof(T)}");
		}
	}
}
