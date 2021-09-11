using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace Sys.Data
{
	static class Iteration
	{
		public static IEnumerable<DataRow> AsEumerableRows(this DataTable dt)
		{
			foreach (DataRow row in dt.Rows)
			{
				yield return row;
			}
		}

		public static IEnumerable<(int, DataRow)> AsEumerableIndexRows(this DataTable dt)
		{
			int i = 0;
			foreach (DataRow row in dt.Rows)
			{
				yield return (i, row);
				i++;
			}
		}

		public static List<T> ToList<T>(this DataTable dt, Func<DataRow, T> select)
		{
			List<T> list = new List<T>();
			foreach (DataRow row in dt.Rows)
			{
				list.Add(select(row));
			}

			return list;
		}

	}
}
