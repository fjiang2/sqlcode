using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace Sys.Data
{
	public abstract class ParameterFacet : IParameterFacet
	{
		public ParameterFacet()
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
		public static IParameterFacet Create(object parameters)
		{
			if (parameters is IEnumerable<IDataParameter> list)
				return new ParameterOfList(list);

			if (parameters is IDictionary<string, object> dict)
				return new ParameterOfDictionary(dict);

			if (parameters is string)
				throw new NotImplementedException();

			return new ParameterOfObject(parameters);
		}

	}
}