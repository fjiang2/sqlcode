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
	public class UnitTest_Parameters
	{
		class Enitity
		{
			public int Id { get; set; }
			public string City { get; set; }
		}


		[TestMethod]
		public void Test_XML_Parameter()
		{
			// Arguments:
			// in int Id = 20
			// out string City = "Houston"
			XElement xml = new XElement("Parameters",
				new XElement("Parameter", new XAttribute("Name", "Id"), new XAttribute("Value", 20), new XAttribute("Type", DbType.Int32), new XAttribute("Direction", ParameterDirection.Input)),
				new XElement("Parameter", new XAttribute("Name", "City"), new XAttribute("Value", "Houston"), new XAttribute("Type", DbType.String), new XAttribute("Direction", ParameterDirection.Output))
				);


			List<IDataParameter> args = ParameterSerialization.FromXml(xml).ToList();

			Debug.Assert(args[0].ParameterName == "Id" && args[1].ParameterName == "City");
			Debug.Assert(args[0].Value.Equals(20) && args[1].Value.Equals("Houston"));
			Debug.Assert(args[0].Direction == ParameterDirection.Input && args[1].Direction == ParameterDirection.Output);

			args[1].Value = "Austin";

			XElement _xml = ParameterSerialization.ToXml(args);
			XElement city = _xml.Elements().Skip(1).First();
			string name = (string)city.Attribute("Value");
			Debug.Assert(name.Equals("Austin"));
		}

		[TestMethod]
		public void Test_Update_Parameter_From_Result()
		{
			var args = new List<IDataParameter>
			{
				new Parameter("Id", 20),
				new Parameter("City", "Houston") { Direction = ParameterDirection.Output },
			};

			IParameterFactory parameters = ParameterFactory.Create(args);
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

			Debug.Assert(args[0].Value.Equals(20));
			Debug.Assert(args[1].Value.Equals("Austin"));
		}


		[TestMethod]
		public void Test_Object_Parameter()
		{
			var args = new Enitity { Id = 20, City = "Houston" };

			IParameterFactory parameters = ParameterFactory.Create(args);
			List<IDataParameter> items = parameters.CreateParameters();
			items[1].Direction = ParameterDirection.Output;

			Debug.Assert(items[0].ParameterName == "Id" && items[1].ParameterName == "City");
			Debug.Assert(items[0].Value.Equals(20) && items[1].Value.Equals("Houston"));
			Debug.Assert(items[0].Direction == ParameterDirection.Input && items[1].Direction == ParameterDirection.Output);

			var result = new SqlParameter[]
			{
				new SqlParameter("Id", 30),
				new SqlParameter("City", "Austin") {Direction = ParameterDirection.Output},
			};

			parameters.UpdateResult(result);

			Debug.Assert(args.Id == 20);
			Debug.Assert(args.City == "Austin");
		}


		[TestMethod]
		public void Test_XML_Parameter_Serialization()
		{
			var paramters = new List<IDataParameter>
			{
				new Parameter("Id", 20),
				new Parameter("City", "Houston") { Direction = ParameterDirection.Output },
			};

			string text = ParameterSerialization.Serialize(paramters);
			IEnumerable<IDataParameter> parameters = ParameterSerialization.Deserialize(text);
			var items = parameters.ToList();

			Debug.Assert(items[0].ParameterName == "Id" && items[1].ParameterName == "City");
			Debug.Assert(items[0].Value.Equals(20) && items[1].Value.Equals("Houston"));
			Debug.Assert(items[0].Direction == ParameterDirection.Input && items[1].Direction == ParameterDirection.Output);

		}
	}
}
