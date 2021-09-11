using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sys.Data
{
	class ListParameters : ParameterFactory
	{
		private IEnumerable<IDataParameter> parameters;

		public ListParameters(IEnumerable<IDataParameter> parameters)
		{
			this.parameters = parameters;
		}

		public override List<IDataParameter> CreateParameters() => parameters.ToList();

		public override void UpdateResult(IEnumerable<IDataParameter> result)
		{
			foreach (IDataParameter parameter in result)
			{
				string parameterName = GetParameterName(parameter);

				if (parameter.Direction == ParameterDirection.Input)
					continue;

				var found = parameters.FirstOrDefault(x => x.ParameterName == parameterName);
				if (found != null)
					found.Value = parameter.Value;
			}
		}
	}
}

