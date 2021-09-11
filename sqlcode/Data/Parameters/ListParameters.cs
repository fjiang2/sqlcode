using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sys.Data
{
	public class ListParameters : ParameterFactory
	{
		private List<IDataParameter> parameters;

		public ListParameters(IEnumerable<IDataParameter> parameters)
		{
			if (parameters is List<IDataParameter> list)
				this.parameters = list;
			else
				this.parameters = parameters.ToList();
		}

		public override List<IDataParameter> CreateParameters() => parameters;

		public override void UpdateResult(IEnumerable<IDataParameter> result)
		{
			foreach (IDataParameter parameter in result)
			{
				string parameterName = GetParameterName(parameter);

				if (parameter.Direction == ParameterDirection.Input)
					continue;

				var found = parameters.Find(x => x.ParameterName == parameterName);
				if (found != null)
					found.Value = parameter.Value;
			}
		}
	}
}

