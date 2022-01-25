using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.Contracts;

namespace Sys.Data
{
	public abstract class DbAccess : IDbFill, IDbAccess
	{
		public string Description { get; set; } = nameof(DbAccess);

		public DbAccess()
		{
		}

		public abstract int FillDataSet(DataSet dataSet);
		public abstract int FillDataTable(DataTable dataTable, int startRecord, int maxRecords);
		public abstract int ReadDataTable(DataTable dataTable, int startRecord, int maxRecords);
		public abstract int ExecuteNonQuery();
		public abstract object ExecuteScalar();
		public abstract void ExecuteTransaction();

		public DataSet FillDataSet()
		{
			DataSet ds = new DataSet();
			FillDataSet(ds);
			return ds;
		}

		public DataTable FillDataTable() => new DataLand(FillDataSet()).GetTable();
		public List<T> FillDataColumn<T>(int column) => new DataLand(FillDataSet()).GetColumn<T>(column);
		public List<T> FillDataColumn<T>(string columnName) => new DataLand(FillDataSet()).GetColumn<T>(columnName);
		public DataRow FillDataRow() => new DataLand(FillDataSet()).GetRow();
		public object FillObject() => new DataLand(FillDataSet()).GetCell();
		public T FillObject<T>()=> new DataLand(FillDataSet()).GetCell<T>();


		public override string ToString()
		{
			return this.Description;
		}
	}
}
