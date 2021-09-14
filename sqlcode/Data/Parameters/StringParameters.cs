using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Sys.Data.Text;

namespace Sys.Data
{
	class StringParameters : ParameterFactory
	{
		private string text;
		private List<IDataParameter> list;

		public StringParameters(string parameters)
			: base(parameters)
		{
			this.text = parameters;
		}

		public override List<IDataParameter> CreateParameters()
		{
			list = Parameter.Deserialize(text).ToList();
			return list;
		}


		public override void UpdateResult(IEnumerable<IDataParameter> result)
		{
			foreach (IDataParameter parameter in result)
			{
				string parameterName = GetParameterName(parameter);

				if (parameter.Direction == ParameterDirection.Input)
					continue;

				var found = list.FirstOrDefault(x => x.ParameterName == parameterName);
				if (found != null)
					found.Value = parameter.Value;
			}

			base.parameters = Parameter.Serialize(list);
		}
	}
}

