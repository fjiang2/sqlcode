using System;

namespace Sys.Data
{
	public class DbCmdParameter
	{
		public string[] Statements { get; }
		public object Args { get; }

		public DbCmdParameter(string[] statements, object args)
		{
			this.Statements = statements;
			this.Args = args;
		}

		public string Statement => string.Join(Environment.NewLine, Statements);

		public override string ToString()
		{
			return $"{Statement}({Args})";
		}
	}
}
