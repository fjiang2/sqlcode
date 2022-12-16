using Sys.Data.Text;

namespace Sys.Data
{
	public class SqlColumnValuePair : IQueryScript
	{
		internal SqlColumn Column { get; }
		public SqlValue Value { get; set; }


		public SqlColumnValuePair(string columnName, object value)
		{
			this.Column = new SqlColumn(columnName, value?.GetType());
			this.Value = new SqlValue(value);
		}

		public string ColumnName => Column.Name;

		public string ToScript(DbAgentStyle style)
		{
			return string.Format("{0} = {1}", Column.ToScript(style), Value.ToScript(style));
		}

		public override string ToString()
		{
			return ToScript(DbAgentOption.DefaultStyle);
		}

		internal BinaryExpression Reduce(string method)
		{
			return new BinaryExpression(new Expression(new ColumnName(Column.Name)), method, new Expression(Value));
		}
	}
}
