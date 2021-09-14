using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Sys.Data.Text;

namespace Sys.Data
{
	/// <summary>
	/// XML example:
	/// <Parameters>
	///   <Parameter Name="Id" Value="20" Type="int" Direction="Input" />
	///   <Parameter Name="City" Value="20" Type="string" Direction="Output" />
	/// </Parameters> 
	/// </summary>
	class XmlParameters : ParameterFactory
	{
		private const string NAME = "Name";
		private const string VALUE = "Value";
		private const string TYPE = "Type";
		private const string DIRECTION = "Direction";

		private XElement parameters;

		public XmlParameters(XElement parameters)
		{
			this.parameters = parameters;
		}

		public override List<IDataParameter> CreateParameters()
		{
			List<IDataParameter> list = new List<IDataParameter>();

			foreach (var element in parameters.Elements())
			{
				string name = (string)element.Attribute(NAME);
				string _value = (string)element.Attribute(VALUE);
				string _type = (string)element.Attribute(TYPE);
				string _direction = (string)element.Attribute(DIRECTION);

				DbType type;
				if (!Enum.TryParse(_type, out type))
					type = DbType.String;

				ParameterDirection direction;
				if (!Enum.TryParse(_direction, out direction))
					direction = ParameterDirection.Input;

				object value = Parse(_value, type);
				var parameter = new Parameter(name, value)
				{
					DbType = type,
					Direction = direction,
				};

				list.Add(parameter);
			}

			return list;
		}

		public override void UpdateResult(IEnumerable<IDataParameter> result)
		{
			foreach (IDataParameter parameter in result)
			{
				string parameterName = GetParameterName(parameter);

				if (parameter.Direction == ParameterDirection.Input)
					continue;

				XElement element = parameters.Elements().FirstOrDefault(x => (string)x.Attribute(NAME) == parameterName);
				if (element != null)
				{
					element.Attribute(VALUE).SetValue(parameter.Value);
				}
			}
		}

		private static object Parse(string text, DbType type)
		{
			switch (type)
			{
				case DbType.AnsiString:
				case DbType.String:
				case DbType.StringFixedLength:
				case DbType.AnsiStringFixedLength:
					return text;

				case DbType.Boolean:
					return bool.Parse(text);

				case DbType.Int16:
					return short.Parse(text);

				case DbType.Int32:
					return int.Parse(text);

				case DbType.Int64:
					return long.Parse(text);

				case DbType.UInt16:
					return ushort.Parse(text);

				case DbType.UInt32:
					return uint.Parse(text);

				case DbType.UInt64:
					return ulong.Parse(text);

				case DbType.Single:
					return float.Parse(text);

				case DbType.Double:
					return double.Parse(text);

				case DbType.Currency:
				case DbType.Decimal:
					return decimal.Parse(text);

				case DbType.Byte:
					return byte.Parse(text);

				case DbType.SByte:
					return sbyte.Parse(text);

				case DbType.DateTime:
				case DbType.DateTime2:
				case DbType.Time:
				case DbType.Date:
					return DateTime.Parse(text);

				case DbType.DateTimeOffset:
					return DateTimeOffset.Parse(text);

				case DbType.Guid:
					return Guid.Parse(text);

				case DbType.Binary:
					return new byte[] { };

				case DbType.Xml:
					return XElement.Parse(text);

				case DbType.VarNumeric:
				case DbType.Object:
					break;
			}

			throw new NotImplementedException();
		}
	}
}

