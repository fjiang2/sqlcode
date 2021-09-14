using System;
using System.Data;
using System.Collections.Generic;
using System.Xml.Linq;

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

			DbType = DbType.String;
			if (value is int)
				DbType = DbType.Int32;
			else if (value is short)
				DbType = DbType.Int16;
			else if (value is long)
				DbType = DbType.Int64;
			if (value is uint)
				DbType = DbType.UInt32;
			else if (value is ushort)
				DbType = DbType.UInt16;
			else if (value is ulong)
				DbType = DbType.UInt64;
			else if (value is byte)
				DbType = DbType.Byte;
			else if (value is sbyte)
				DbType = DbType.SByte;
			else if (value is DateTime)
				DbType = DbType.DateTime;
			else if (value is double)
				DbType = DbType.Double;
			else if (value is float)
				DbType = DbType.Single;
			else if (value is decimal)
				DbType = DbType.Decimal;
			else if (value is bool)
				DbType = DbType.Boolean;
			else if (value is string && ((string)value).Length > 4000)
				DbType = DbType.AnsiString;
			else if (value is string)
				DbType = DbType.String;
			else if (value is byte[])
				DbType = DbType.Binary;
			else if (value is Guid)
				DbType = DbType.Guid;
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


		public static XElement ToXElement(IEnumerable<IDataParameter> parameters)
		{
			return ParameterSerialization.ToXElement(parameters);
		}

		public static IEnumerable<IDataParameter> ToParameters(XElement parameters)
		{
			return ParameterSerialization.ToParameters(parameters);
		}

		public static string Serialize(IEnumerable<IDataParameter> parameters)
		{
			string xml = ToXElement(parameters).ToString();
			var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
			return Convert.ToBase64String(bytes);
		}

		public static IEnumerable<IDataParameter> Deserialize(string text)
		{
			var bytes = Convert.FromBase64String(text);
			string xml = System.Text.Encoding.UTF8.GetString(bytes);

			XElement element = XElement.Parse(xml);
			return ToParameters(element);
		}
	}
}
