using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sys.Data
{
	public class ListParameters : IParameterFactory
	{
		private List<IDataParameter> parameters;

		public ListParameters(IEnumerable<IDataParameter> parameters)
		{
			if (parameters is List<IDataParameter> list)
				this.parameters = list;
			else
				this.parameters = parameters.ToList();
		}

		public List<IDataParameter> Create()
		{
			return parameters;
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

				var found = parameters.Find(x => x.ParameterName == parameterName);
				if (found != null)
					found.Value = parameter.Value;
			}
		}
	}
}

