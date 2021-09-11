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

		public DataTable FillDataTable()
		{
			int table = 0;

			DataSet ds = FillDataSet();
			if (ds == null)
				return null;

			if (table < ds.Tables.Count)
				return ds.Tables[table];

			return null;
		}

		public List<T> FillDataColumn<T>(int column = 0)
		{
			Contract.Requires(column >= 0);

			List<T> list = new List<T>();

			DataTable _table = FillDataTable();
			if (_table == null)
				return list;

			foreach (DataRow row in _table.Rows)
			{
				object obj = row[column];
				list.Add(Convert<T>(obj));
			}

			return list;
		}

		public List<T> FillDataColumn<T>(string columnName)
		{
			Contract.Requires(!string.IsNullOrEmpty(columnName));

			List<T> list = new List<T>();

			DataTable _table = FillDataTable();
			if (_table == null)
				return list;

			foreach (DataRow row in _table.Rows)
			{
				object obj = row[columnName];
				list.Add(Convert<T>(obj));
			}

			return list;
		}

		public DataRow FillDataRow()
		{
			int row = 0;

			DataTable _table = FillDataTable();
			if (_table != null && row < _table.Rows.Count)
				return _table.Rows[row];
			else
				return null;
		}

		public object FillObject()
		{
			int column = 0;
			DataRow _row = FillDataRow();
			if (_row != null && column < _row.Table.Columns.Count)
				return _row[column];
			else
				return null;
		}

		public T FillObject<T>()
		{
			var obj = FillObject();

			return Convert<T>(obj);
		}

	
		private static T Convert<T>(object obj)
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
