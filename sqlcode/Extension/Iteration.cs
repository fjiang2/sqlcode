using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace Sys.Data
{
	static class Iteration
	{
		public static IEnumerable<DataRow> AsEnumerableRows(this DataTable dt)
		{
			foreach (DataRow row in dt.Rows)
			{
				yield return row;
			}
		}

		public static IEnumerable<(int, DataRow)> AsEnumerableIndexRows(this DataTable dt)
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

		public static IEnumerable<List<T>> Split<T>(this IEnumerable<T> source, int batchSize)
		{
			int count = 0;
			List<T> list = new List<T>();

			foreach (T item in source)
			{
				list.Add(item);

				count++;
				if (count >= batchSize)
				{
					yield return list;
					list.Clear();
					count = 0;
				}
			}

			yield return list;
		}

	}
}
