using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Sys.Data.Text;

namespace Sys.Data
{
	public class ObjectParameters : ParameterFactory
	{
		private object parameters;
		public ObjectParameters(object parameters)
		{
			this.parameters = parameters;
		}

		public override List<IDataParameter> CreateParameters()
		{
			List<IDataParameter> list = new List<IDataParameter>();
			foreach (var propertyInfo in parameters.GetType().GetProperties())
			{
				object value = propertyInfo.GetValue(parameters) ?? DBNull.Value;
				var p = new Parameter(propertyInfo.Name, value);
				list.Add(p);
			}

			return list;
		}


		public override void UpdateResult(IEnumerable<IDataParameter> result)
		{
			var properties = parameters.GetType().GetProperties();
			foreach (IDataParameter parameter in result)
			{
				string parameterName = GetParameterName(parameter);

				if (parameter.Direction == ParameterDirection.Input)
					continue;

				var found = properties.FirstOrDefault(property => property.Name == parameterName);
				if (found != null)
				{
					found.SetValue(parameters, parameter.Value);
				}
			}
		}
	}
}

