using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace Sys.Data
{
	public abstract class ParameterFactory : IParameterFactory
	{
		public ParameterFactory()
		{
		}

		protected string GetParameterName(IDataParameter parameter)
		{
			string parameterName = parameter.ParameterName;

			if (parameterName.StartsWith("@") || parameterName.StartsWith("$"))
				parameterName = parameterName.Substring(1);

			return parameterName;
		}

		public abstract List<IDataParameter> CreateParameters();

		public abstract void UpdateResult(IEnumerable<IDataParameter> result);

		
		public static string Serialize(IEnumerable<IDataParameter> parameters)
		{
			string xml = ParameterSerialization.ToXElement(parameters).ToString();
			var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
			return Convert.ToBase64String(bytes);
		}

		public static IEnumerable<IDataParameter> Deserialize(string text)
		{
			var bytes = Convert.FromBase64String(text);
			string xml = System.Text.Encoding.UTF8.GetString(bytes);

			XElement element = XElement.Parse(xml);
			return ParameterSerialization.ToParameters(element);
		}

		public static XElement ToXElement(IEnumerable<IDataParameter> parameters)
		{
			return ParameterSerialization.ToXElement(parameters);
		}

		public static IEnumerable<IDataParameter> ToParameters(XElement parameters)
		{
			return ParameterSerialization.ToParameters(parameters);
		}


		/// <summary>
		/// Create parameters for SQL statements
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static IParameterFactory Create(object parameters)
		{
			if (parameters is List<IDataParameter> list)
				return new ListParameters(list);

			if (parameters is IDictionary<string, object> dict)
				return new DictionaryParameters(dict);

			if (parameters is string)
				throw new NotImplementedException();

			return new ObjectParameters(parameters);
		}

	}
}