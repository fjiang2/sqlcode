using System.Collections.Generic;
using System.Data;

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

		public static IParameterFactory Create(object parameters)
		{
			IParameterFactory factory;

			if (parameters is List<IDataParameter> list)
				factory = new ListParameters(list);
			else if (parameters is IDictionary<string, object> dict)
				factory = new DictionaryParameters(dict);
			else
				factory = new ObjectParameters(parameters);

			return factory;
		}

	}
}