using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sys.Data;
using Sys.Data.Text;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest_Others
	{
		[TestMethod]
		public void Test_XML_Parameter()
		{
			// Arguments:
			// in int Id = 20
			// out string City = "Houston"

			XElement element = new XElement("Parameters",
				new XElement("Parameter", new XAttribute("Name", "Id"), new XAttribute("Value", 20), new XAttribute("Type", DbType.Int32), new XAttribute("Direction", ParameterDirection.Input)),
				new XElement("Parameter", new XAttribute("Name", "City"), new XAttribute("Value", "Houston"), new XAttribute("Type", DbType.String), new XAttribute("Direction", ParameterDirection.Output))
				);

			string _args = element.ToString();
			XElement args = XElement.Parse(_args);

			var parameters = ParameterFactory.Create(args);
			List<IDataParameter> items = parameters.CreateParameters();

			Debug.Assert(items[0].ParameterName == "Id" && items[1].ParameterName == "City");
			Debug.Assert(items[0].Value.Equals(20) && items[1].Value.Equals("Houston"));
			Debug.Assert(items[0].Direction == ParameterDirection.Input && items[1].Direction == ParameterDirection.Output);

			var result = new SqlParameter[]
				{
					new SqlParameter("Id", 30),
					new SqlParameter("City", "Austin") {Direction = ParameterDirection.Output},
				};
			
			parameters.UpdateResult(result);

			XElement city = args.Elements().Skip(1).First();
			string name = (string)city.Attribute("Value");
			Debug.Assert(name.Equals("Austin"));
		}

		[TestMethod]
		public void Test_XML_Parameter2()
		{
			var paramters = new List<Parameter>
			{
				new Parameter("Id", 20),
				new Parameter("City", "Houston") { Direction = ParameterDirection.Output },
			};
			XElement args = Parameter.ToXElement(paramters);

			var parameters = ParameterFactory.Create(args);
			List<IDataParameter> items = parameters.CreateParameters();

			Debug.Assert(items[0].ParameterName == "Id" && items[1].ParameterName == "City");
			Debug.Assert(items[0].Value.Equals(20) && items[1].Value.Equals("Houston"));
			Debug.Assert(items[0].Direction == ParameterDirection.Input && items[1].Direction == ParameterDirection.Output);

			var result = new SqlParameter[]
				{
					new SqlParameter("Id", 30),
					new SqlParameter("City", "Austin") {Direction = ParameterDirection.Output},
				};

			parameters.UpdateResult(result);

			var L = Parameter.ToParameters(args).ToArray();
			Debug.Assert(L[1].Value.Equals("Austin"));
		}

		[TestMethod]
		public void Test_XML_Parameter_Serialization()
		{
			var paramters = new List<Parameter>
			{
				new Parameter("Id", 20),
				new Parameter("City", "Houston") { Direction = ParameterDirection.Output },
			};

			string text = Parameter.Serialize(paramters);
			IEnumerable<IDataParameter> parameters = Parameter.Deserialize(text);
			var items = parameters.ToList();

			Debug.Assert(items[0].ParameterName == "Id" && items[1].ParameterName == "City");
			Debug.Assert(items[0].Value.Equals(20) && items[1].Value.Equals("Houston"));
			Debug.Assert(items[0].Direction == ParameterDirection.Input && items[1].Direction == ParameterDirection.Output);

		}
	}
}
