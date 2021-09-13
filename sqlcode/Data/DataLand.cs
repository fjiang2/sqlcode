using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.Contracts;

namespace Sys.Data
{
	class DataLand
	{
		private readonly DataSet dataSet;
		public DataLand(DataSet dataSet)
		{
			this.dataSet = dataSet;
		}

		public DataSet GetDataSet()
		{
			return dataSet;
		}

		public DataTable GetTable(int table = 0)
		{
			Contract.Requires(table >= 0);

			DataSet ds = GetDataSet();
			if (ds == null)
				return null;

			if (table < ds.Tables.Count)
				return ds.Tables[table];

			return null;
		}

		public List<T> GetColumn<T>(int column = 0, int table = 0)
		{
			Contract.Requires(table >= 0 && column >= 0);

			List<T> list = new List<T>();

			DataTable _table = GetTable(table);
			if (_table == null)
				return list;

			foreach (DataRow row in _table.Rows)
			{
				object obj = row[column];
				list.Add(IsNull<T>(obj));
			}

			return list;
		}

		public List<T> GetColumn<T>(string columnName, int table = 0)
		{
			Contract.Requires(table >= 0 && !string.IsNullOrEmpty(columnName));

			List<T> list = new List<T>();

			DataTable _table = GetTable(table);
			if (_table == null)
				return list;

			foreach (DataRow row in _table.Rows)
			{
				object obj = row[columnName];
				list.Add(IsNull<T>(obj));
			}

			return list;
		}

		public DataRow GetRow(int row = 0, int table = 0)
		{
			Contract.Requires(table >= 0 && row >= 0);

			DataTable _table = GetTable(table);
			if (_table != null && row < _table.Rows.Count)
				return _table.Rows[row];
			else
				return null;
		}

		public object GetCell(int column = 0, int row = 0, int table = 0)
		{
			Contract.Requires(table >= 0 && row >= 0 && column >= 0);

			DataRow _row = GetRow(row, table);
			if (_row != null && column < _row.Table.Columns.Count)
				return _row[column];
			else
				return null;
		}

		public T GetCell<T>(int column = 0, int row = 0, int table = 0)
		{
			Contract.Requires(table >= 0 && row >= 0 && column >= 0);

			var obj = GetCell(column, row, table);

			return IsNull<T>(obj);
		}

		private static T IsNull<T>(object value)
		{
			if (value == null || value == DBNull.Value)
				return default(T);

			if (value is T)
				return (T)value;

			throw new Exception($"{value} is not type of {typeof(T)}");
		}

	}
}
