using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Sys.Data.Text;

namespace Sys.Data
{
	public class ObjectParameters : IParameterFactory
	{
		private object parameters;
		public ObjectParameters(object parameters)
		{
			this.parameters = parameters;
		}

		public List<IDataParameter> Create()
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

		public void Update(IEnumerable<IDataParameter> result)
		{
			var properties = parameters.GetType().GetProperties();
			foreach (IDataParameter parameter in result)
			{
				string parameterName = parameter.ParameterName;
				if (parameterName.StartsWith("@"))
					parameterName = parameterName.Substring(1);

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

