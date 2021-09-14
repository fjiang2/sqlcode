using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Sys.Data.Text;

namespace Sys.Data
{
	class ObjectParameters : ParameterFactory
	{
		private object obj;
		public ObjectParameters(object parameters)
			: base(parameters)
		{
			this.obj = parameters;
		}

		public override List<IDataParameter> CreateParameters()
		{
			List<IDataParameter> list = new List<IDataParameter>();

			foreach (var propertyInfo in obj.GetType().GetProperties())
			{
				object value = propertyInfo.GetValue(obj);
				var parameter = new Parameter(propertyInfo.Name, value)
				{
					Direction = ParameterDirection.Input,
				};

				list.Add(parameter);
			}

			return list;
		}


		public override void UpdateResult(IEnumerable<IDataParameter> result)
		{
			var properties = obj.GetType().GetProperties();
			foreach (IDataParameter parameter in result)
			{
				string parameterName = GetParameterName(parameter);

				if (parameter.Direction == ParameterDirection.Input)
					continue;

				var found = properties.FirstOrDefault(property => property.Name == parameterName);
				if (found != null)
				{
					found.SetValue(obj, parameter.Value);
				}
			}
		}
	}
}

