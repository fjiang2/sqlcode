using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace Sys.Data
{
	public abstract class ParameterFactory : IParameterFactory
	{
		protected object parameters;
		public ParameterFactory(object parameters)
		{
			this.parameters = parameters;
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

		public object Parameters => this.parameters;

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

			if (parameters is XElement element)
				return new XmlParameters(element);

			if (parameters is string xml)
				return new StringParameters(xml);

			return new ObjectParameters(parameters);
		}

	}
}