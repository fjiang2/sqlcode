using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sys.Data
{
    /// <summary>
    /// Represent a collection of column/value pairs
    /// </summary>
    public class SqlColumnValuePairCollection : IEnumerable<SqlColumnValuePair>
    {
        protected List<SqlColumnValuePair> columns = new List<SqlColumnValuePair>();

        public SqlColumnValuePairCollection()
        {
        }

        public IEnumerator<SqlColumnValuePair> GetEnumerator() => columns.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()=> ((IEnumerable)columns).GetEnumerator();

        /// <summary>
        /// Clear all column/value pairs
        /// </summary>
        public void Clear()
        {
            columns.Clear();
        }

        /// <summary>
        /// Add all properties of data contract class
        /// </summary>
        /// <param name="data"></param>
        public SqlColumnValuePairCollection AddRange(object data)
        {
            foreach (var propertyInfo in data.GetType().GetProperties())
            {
                object value = propertyInfo.GetValue(data) ?? DBNull.Value;
                Add(propertyInfo.Name, value);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        public SqlColumnValuePairCollection AddRange(IDictionary<string, object> map)
        {
            foreach (var kvp in map)
            {
                Add(kvp.Key, kvp.Value);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        public SqlColumnValuePairCollection AddRange(DataRow row)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                Add(column.ColumnName, row[column]);
            }
            
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="version"></param>
        public SqlColumnValuePairCollection AddRange(DataRow row, DataRowVersion version)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                Add(column.ColumnName, row[column, version]);
            }

            return this;
        }

        /// <summary>
        /// Add col0,col1,... 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnPrefix"></param>
        /// <param name="values"></param>
        public SqlColumnValuePairCollection AddRange<T>(string columnPrefix, T[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                string column = $"{columnPrefix}{i + 1}";
                Add(column, values[i]);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnPrefix"></param>
        /// <param name="values"></param>
        public SqlColumnValuePairCollection AddRange(string columnPrefix, object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                string column = $"{columnPrefix}{i + 1}";
                Add(column, values[i]);
            }

            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="values"></param>
        public SqlColumnValuePairCollection AddRange(string[] columns, object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Add(columns[i], values[i]);
            }

            return this;
        }

        public virtual SqlColumnValuePair Add(string name, object value)
        {
            SqlColumnValuePair found = columns.Find(c => c.ColumnName == name);
            if (found != null)
            {
                found.Value = new SqlValue(value);
                return found;
            }
            else
            {
                var pair = new SqlColumnValuePair(name, value);
                columns.Add(pair);
                return pair;
            }
        }

        public int RemoveRange(IEnumerable<string> columns)
        {
            int count = 0;
            foreach (var column in columns)
            {
                if (Remove(column))
                    count++;
            }

            return count;
        }

        public bool Remove(string column)
        {
            SqlColumnValuePair found = columns.Find(c => c.ColumnName == column);
            if (found != null)
                return columns.Remove(found);

            return false;
        }

        public IDictionary<string, object> ToDictionary()
        {
            return columns.ToDictionary(c => c.ColumnName, c => c.Value.Value);
        }


        /// <summary>
        /// To SQL column/value list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SqlColumnValuePair> ToList()
        {
            return columns;
        }

        public string Join(Func<SqlColumnValuePair, string> expr, string separator)
        {
            var L = columns.Select(pair => expr(pair));
            return string.Join(separator, L);
        }

        public string Join(string separator)
        {
            return Join(pair => $"{pair.ColumnFormalName} = {pair.Value}", separator);
        }

        internal IEnumerable<Text.BinaryExpression> Reduce(string method)
		{
            return ToList().Select(pair => pair.Reduce(method));
        }

        public override string ToString()
        {
            return columns.ToString();
        }

	}

}
