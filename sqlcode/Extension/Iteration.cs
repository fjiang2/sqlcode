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

		public static void ForEach<TSource>(this IEnumerable<TSource> items, Action<TSource> action, Action<TSource> delimiter)
		{
			bool first = true;

			foreach (var item in items)
			{
				if (!first)
					delimiter(item);

				first = false;
				action(item);
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
