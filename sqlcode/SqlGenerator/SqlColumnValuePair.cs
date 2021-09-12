using Sys.Data.Text;

namespace Sys.Data
{
	public class SqlColumnValuePair
	{
		internal SqlColumn Column { get; }
		public SqlValue Value { get; set; }


		public SqlColumnValuePair(string columnName, object value)
		{
			this.Column = new SqlColumn(columnName, value?.GetType());
			this.Value = new SqlValue(value);
		}

		public string ColumnName => Column.Name;

		internal string ColumnFormalName => FormalName(ColumnName);

		public string ToScript(DbAgentStyle style)
		{
			return string.Format("[{0}] = {1}", ColumnName, Value.ToScript(style));
		}

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

		internal BinaryExpression Reduce(string method)
		{
			return new BinaryExpression(new Expression(new ColumnName(Column.Name)), method, new Expression(Value));
		}
	}
}
