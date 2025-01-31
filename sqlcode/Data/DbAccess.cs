using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Sys.Data.Text;

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
        public abstract int ReadDataSet(DataSet dataSet);
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

        public DataSet ReadDataSet()
        {
            DataSet ds = new DataSet();
            ReadDataSet(ds);
            return ds;
        }

        public DataTable FillDataTable() => new DataLand(FillDataSet()).GetTable();
        public List<T> FillDataColumn<T>(int column) => new DataLand(FillDataSet()).GetColumn<T>(column);
        public List<T> FillDataColumn<T>(string columnName) => new DataLand(FillDataSet()).GetColumn<T>(columnName);
        public DataRow FillDataRow() => new DataLand(FillDataSet()).GetRow();
        public object FillObject() => new DataLand(FillDataSet()).GetCell();
        public T FillObject<T>() => new DataLand(FillDataSet()).GetCell<T>();

        public DataTable ReadDataTable() => new DataLand(ReadDataSet()).GetTable();
        public List<T> ReadDataColumn<T>(int column) => new DataLand(ReadDataSet()).GetColumn<T>(column);
        public List<T> ReadDataColumn<T>(string columnName) => new DataLand(ReadDataSet()).GetColumn<T>(columnName);
        public DataRow ReadDataRow() => new DataLand(ReadDataSet()).GetRow();
        public object ReadObject() => new DataLand(ReadDataSet()).GetCell();
        public T ReadObject<T>() => new DataLand(ReadDataSet()).GetCell<T>();

        public List<T> ToList<T>() where T : new()
        {
            var dt = FillDataTable();

            List<T> list = new List<T>();

            var properties = typeof(T).GetProperties();
            foreach (DataRow row in dt.Rows)
            {
                T item = new T();
                foreach (var property in properties)
                {
                    string columnName = property.Name;
                    if (row.Table.Columns.Contains(columnName))
                    {
                        var value = row[columnName];
                        if (value != DBNull.Value)
                        {
                            property.SetValue(item, value);
                        }
                    }
                }

                list.Add(item);
            }

            return list;
        }

        public List<T> ToList<T>(Func<DataRow, T> select)
        {
            var dt = FillDataTable();
            return dt.ToList<T>(select);
        }

        public override string ToString()
        {
            return this.Description;
        }
    }
}
