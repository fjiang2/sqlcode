namespace Sys.Data
{
    public class SqlColumnValuePair
    {
        internal SqlColumn Column { get; }
        internal SqlValue Value { get; set; }


        public SqlColumnValuePair(string columnName, object value)
        {
            this.Column = new SqlColumn(columnName, value?.GetType());
            this.Value = new SqlValue(value);
        }

        public string ColumnName => Column.Name;

        public string ColumnFormalName => FormalName(ColumnName);

        public override string ToString()
        {
            return string.Format("[{0}] = {1}", ColumnName, Value);
        }

        internal static string FormalName(string name)
        {
            if (name.StartsWith("[") && name.EndsWith("]"))
                return name;

            return $"[{name}]";
        }

    }
}
