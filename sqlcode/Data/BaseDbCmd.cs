using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.Contracts;

namespace Sys.Data
{
	public abstract class BaseDbCmd : IDbFill, IDbCmd
	{
		public string Description { get; set; } = nameof(BaseDbCmd);

		public BaseDbCmd()
		{
		}

		public abstract DataSet FillDataSet(DataSet dataSet);
		public abstract int ExecuteNonQuery();
		public abstract object ExecuteScalar();

		public DataSet FillDataSet()
		{

			DataSet ds = new DataSet();

			if (FillDataSet(ds) == null)
				return null;

			return ds;
		}

		public DataTable FillDataTable(int table = 0)
		{
			DataSet ds = FillDataSet();
			if (ds == null)
				return null;

			if (table < ds.Tables.Count)
				return ds.Tables[table];

			return null;
		}

		/// <summary>
		/// Get list from specified column in a table
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="column"></param>
		/// <param name="table">table id from 0 to max-1</param>
		/// <returns></returns>
		public List<T> FillDataColumn<T>(int column = 0, int table = 0)
		{
			Contract.Requires(column >= 0);

			List<T> list = new List<T>();

			DataTable _table = FillDataTable(table);
			if (_table == null)
				return list;

			foreach (DataRow row in _table.Rows)
			{
				object obj = row[column];
				list.Add(ToObject<T>(obj));
			}

			return list;
		}

		public List<T> FillDataColumn<T>(string columnName, int table = 0)
		{
			Contract.Requires(!string.IsNullOrEmpty(columnName));

			List<T> list = new List<T>();

			DataTable _table = FillDataTable(table);
			if (_table == null)
				return list;

			foreach (DataRow row in _table.Rows)
			{
				object obj = row[columnName];
				list.Add(ToObject<T>(obj));
			}

			return list;
		}

		public DataRow FillDataRow()
		{
			return FillDataRow(row: 0, table: 0);
		}

		public DataRow FillDataRow(int row = 0, int table = 0)
		{
			Contract.Requires(row >= 0);

			DataTable _table = FillDataTable(table);
			if (_table != null && row < _table.Rows.Count)
				return _table.Rows[row];
			else
				return null;
		}

		public object FillObject(int column = 0, int row = 0, int table = 0)
		{
			DataRow _row = FillDataRow(row, table);
			if (_row != null && column < _row.Table.Columns.Count)
				return _row[column];
			else
				return null;
		}

		public T FillObject<T>(int column = 0, int row = 0, int table = 0)
		{
			var obj = FillObject(column, row, table);

			return ToObject<T>(obj);
		}

		public object FillObject(string column, int row = 0, int table = 0)
		{
			DataRow _row = FillDataRow(row, table);
			if (_row != null && _row.Table.Columns.Contains(column))
				return _row[column];
			else
				return null;
		}

		public T FillObject<T>(string column, int row = 0, int table = 0)
		{
			var obj = FillObject(column, row, table);

			return ToObject<T>(obj);
		}

		private static T ToObject<T>(object obj)
		{
			if (obj != null && obj != DBNull.Value)
				return (T)obj;
			else
				return default(T);
		}

		public override string ToString()
		{
			return this.Description;
		}
	}
}
