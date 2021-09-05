using System;
using System.Data;

namespace Sys.Data.Text
{
	public class Parameter : IDataParameter
	{
		public DbType DbType { get; set; }
		public bool IsNullable { get; set; }
		public string SourceColumn { get; set; }
		public DataRowVersion SourceVersion { get; set; } = DataRowVersion.Current;

		public string ParameterName { get; set; }
		public object Value { get; set; }
		public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

		public Parameter(string parameterName, object value)
		{
			this.ParameterName = parameterName;
			this.Value = value ?? DBNull.Value;
		}

		public override string ToString()
		{
			string modifier = null;
			switch (Direction)
			{
				case ParameterDirection.Input:
					modifier = "in";
					break;

				case ParameterDirection.Output:
					modifier = "out";
					break;

				case ParameterDirection.InputOutput:
					modifier = "ref";
					break;

				case ParameterDirection.ReturnValue:
					modifier = "ret";
					break;
			}

			if (modifier != null)
				return $"{modifier} @{ParameterName} = {Value}";
			else
				return $"@{ParameterName} = {Value}";
		}
	}
}
