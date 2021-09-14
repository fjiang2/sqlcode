using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Sys.Data.Text;

namespace Sys.Data
{
	/// <summary>
	/// XML example:
	/// <Parameters>
	///   <Parameter Name="Id" Value="20" Type="int" Direction="Input" />
	///   <Parameter Name="City" Value="20" Type="string" Direction="Output" />
	/// </Parameters> 
	/// </summary>
	class XmlParameters : ParameterFactory
	{
		private XElement xelement;

		public XmlParameters(XElement parameters)
			: base(parameters)
		{
			this.xelement = parameters;
		}

		public override List<IDataParameter> CreateParameters()
		{
			return ParameterSerialization.ToParameters(xelement);
		}

		public override void UpdateResult(IEnumerable<IDataParameter> result)
		{
			foreach (IDataParameter parameter in result)
			{
				string parameterName = GetParameterName(parameter);

				if (parameter.Direction == ParameterDirection.Input)
					continue;

				XElement element = xelement.Elements().FirstOrDefault(x => (string)x.Attribute(ParameterSerialization._NAME) == parameterName);
				if (element != null)
				{
					element.Attribute(ParameterSerialization._VALUE).SetValue(parameter.Value);
				}
			}
		}
	}
}

