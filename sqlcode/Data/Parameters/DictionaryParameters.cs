using System;
using System.Collections.Generic;
using System.Data;
using Sys.Data.Text;

namespace Sys.Data
{
	public class DictionaryParameters : ParameterFactory
	{
		private IDictionary<string, object> parameters;

		public DictionaryParameters(IDictionary<string, object> parameters)
		{
			this.parameters = parameters;
		}

		public override List<IDataParameter> CreateParameters()
		{
			List<IDataParameter> list = new List<IDataParameter>();

			foreach (KeyValuePair<string, object> item in parameters)
			{
				object value = item.Value ?? DBNull.Value;
				var parameter = new Parameter(item.Key, value)
				{
					Direction = ParameterDirection.Input,
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

				if (parameters.ContainsKey(parameterName))
				{
					parameters[parameterName] = parameter.Value;
				}
			}
		}
	}
}

