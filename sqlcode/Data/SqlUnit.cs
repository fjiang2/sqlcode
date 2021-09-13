using System;

namespace Sys.Data
{
	/// <summary>
	/// Define SQL statements with arguments
	/// </summary>
	public class SqlUnit
	{
		/// <summary>
		/// SQL statements which could be SELECT, INSERT, UPDATE, DELETE statements
		/// </summary>
		public string[] Statements { get; }

		/// <summary>
		/// SQL arguments, which can be a list of IDataParameter, a dictionary, an instance of class, and JSON/XML string
		/// </summary>
		public object Arguments { get; set; }

		public SqlUnit(string[] statements)
		{
			this.Statements = statements;
		}

		public SqlUnit(string statement)
		{
			this.Statements = new string[] { statement };
		}

		public SqlUnit(string statement, object args)
		{
			this.Statements = new string[] { statement };
			this.Arguments = args;
		}

		/// <summary>
		/// Combine all SQL statement into a single query
		/// </summary>
		public string Statement => string.Join(Environment.NewLine, Statements);

		public override string ToString()
		{
			return $"{Statement}({Arguments})";
		}
	}
}
