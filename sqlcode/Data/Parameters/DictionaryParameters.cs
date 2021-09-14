using System;
using System.Collections.Generic;
using System.Data;
using Sys.Data.Text;

namespace Sys.Data
{
	class DictionaryParameters : ParameterFactory
	{
		private IDictionary<string, object> dict;

		public DictionaryParameters(IDictionary<string, object> parameters)
			: base(parameters)
		{
			this.dict = parameters;
		}

		public override List<IDataParameter> CreateParameters()
		{
			List<IDataParameter> list = new List<IDataParameter>();

			foreach (KeyValuePair<string, object> item in dict)
			{
				object value = item.Value;
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

				if (dict.ContainsKey(parameterName))
				{
					dict[parameterName] = parameter.Value;
				}
			}
		}
	}
}

