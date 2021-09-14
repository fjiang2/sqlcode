﻿using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sys.Data
{
	class ListParameters : ParameterFactory
	{
		private IEnumerable<IDataParameter> list;

		public ListParameters(IEnumerable<IDataParameter> parameters)
		{
			this.list = parameters;
		}

		public override List<IDataParameter> CreateParameters() => list.ToList();

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
		}
	}
}

