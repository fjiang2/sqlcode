using System;
using System.Collections.Generic;
using System.Data;
using Sys.Data.Text;

namespace Sys.Data
{
	public class DictionaryParameters : IParameterFactory
	{
		private IDictionary<string, object> parameters;

		public DictionaryParameters(IDictionary<string, object> parameters)
		{
			this.parameters = parameters;
		}

		public List<IDataParameter> Create()
		{
			List<IDataParameter> list = new List<IDataParameter>();
			foreach (KeyValuePair<string, object> item in parameters)
			{
				object value = item.Value ?? DBNull.Value;
				var p = new Parameter(item.Key, value);
				list.Add(p);
			}

			return list;
		}


		public void Update(IEnumerable<IDataParameter> result)
		{
			foreach (IDataParameter parameter in result)
			{
				string parameterName = parameter.ParameterName;
				if (parameterName.StartsWith("@"))
					parameterName = parameterName.Substring(1);

				if (parameter.Direction == ParameterDirection.Input)
					continue;

				if (parameters.ContainsKey(parameterName))
				{
					parameters[parameterName] = parameter.Value;
				}
			}
		}
	}
}

