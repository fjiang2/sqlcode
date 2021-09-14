using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml.Linq;
using Sys.Data.Text;

namespace Sys.Data
{
	class ParameterSerialization
	{
		private const string _PARAMETERS = "Parameters";
		private const string _PARAMETER = "Parameter";
		public const string _NAME = "Name";
		public const string _VALUE = "Value";
		private const string _TYPE = "Type";
		private const string _DIRECTION = "Direction";

		public static XElement ToXElement(IEnumerable<IDataParameter> parameters)
		{
			XElement element = new XElement(_PARAMETERS);
			foreach (var parameter in parameters)
			{
				element.Add(ToXElement(parameter));
			}

			return element;
		}

		private static XElement ToXElement(IDataParameter parameter)
		{
			return new XElement(_PARAMETER,
				new XAttribute(_NAME, parameter.ParameterName),
				new XAttribute(_VALUE, parameter.Value),
				new XAttribute(_TYPE, parameter.DbType),
				new XAttribute(_DIRECTION, parameter.Direction)
				);
		}

		
		public static List<IDataParameter> ToParameters(XElement xml)
		{
			List<IDataParameter> list = new List<IDataParameter>();
			foreach (var element in xml.Elements())
			{
				list.Add(ToParameter(element));
			}

			return list;
		}

		private static IDataParameter ToParameter(XElement xml)
		{
			string name = (string)xml.Attribute(_NAME);
			string _value = (string)xml.Attribute(_VALUE);
			string _type = (string)xml.Attribute(_TYPE);
			string _direction = (string)xml.Attribute(_DIRECTION);

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

			return parameter;

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
