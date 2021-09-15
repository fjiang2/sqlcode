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


		/// <summary>
		/// Create parameters for SQL statements
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static IParameterFactory Create(object parameters)
		{
			if (parameters is IEnumerable<IDataParameter> list)
				return new ListParameters(list);

			if (parameters is IDictionary<string, object> dict)
				return new DictionaryParameters(dict);

			if (parameters is string)
				throw new NotImplementedException();

			return new ObjectParameters(parameters);
		}

	}
}