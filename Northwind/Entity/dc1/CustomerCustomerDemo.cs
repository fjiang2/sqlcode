using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc1
{
	public partial class CustomerCustomerDemo
	{
		public string CustomerID { get; set; }
		public string CustomerTypeID { get; set; }
	}
	
	public class CustomerCustomerDemoAssociation
	{
		public EntityRef<Customers> Customer { get; set; }
		public EntityRef<CustomerDemographics> CustomerDemographic { get; set; }
	}
	
	public static class CustomerCustomerDemoExtension
	{
		public const string TableName = "CustomerCustomerDemo";
		public static readonly string[] Keys = new string[] { _CUSTOMERID, _CUSTOMERTYPEID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Customers>
			{
				Name = "FK_CustomerCustomerDemo_Customers",
				ThisKey = _CUSTOMERID,
				OtherKey = CustomersExtension._CUSTOMERID,
				IsForeignKey = true
			},
			new Constraint<CustomerDemographics>
			{
				Name = "FK_CustomerCustomerDemo",
				ThisKey = _CUSTOMERTYPEID,
				OtherKey = CustomerDemographicsExtension._CUSTOMERTYPEID,
				IsForeignKey = true
			}
		};
		
		public static List<CustomerCustomerDemo> ToCustomerCustomerDemoCollection(this DataTable dt)
		{
			return dt.AsEnumerable()
			.Select(row =>
			{
				var obj = new CustomerCustomerDemo();
				FillObject(obj, row);
				return obj;
			})
			.ToList();
		}
		
		public static void FillObject(this CustomerCustomerDemo item, DataRow row)
		{
			item.CustomerID = row.GetField<string>(_CUSTOMERID);
			item.CustomerTypeID = row.GetField<string>(_CUSTOMERTYPEID);
		}
		
		public static void UpdateRow(this CustomerCustomerDemo item, DataRow row)
		{
			row.SetField(_CUSTOMERID, item.CustomerID);
			row.SetField(_CUSTOMERTYPEID, item.CustomerTypeID);
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable { TableName = TableName };
			dt.Columns.Add(new DataColumn(_CUSTOMERID, typeof(string)) { AllowDBNull = false, MaxLength = 5 });
			dt.Columns.Add(new DataColumn(_CUSTOMERTYPEID, typeof(string)) { AllowDBNull = false, MaxLength = 10 });
			
			dt.PrimaryKey = dt.Columns.OfType<DataColumn>().Where(column => Keys.Contains(column.ColumnName)).ToArray();
			
			return dt;
		}
		
		public static void ToDataTable(this IEnumerable<CustomerCustomerDemo> items, DataTable dt)
		{
			foreach (var item in items)
			{
				var row = dt.NewRow();
				UpdateRow(item, row);
				dt.Rows.Add(row);
			}
			dt.AcceptChanges();
		}
		
		public static IDictionary<string, object> ToDictionary(this CustomerCustomerDemo item)
		{
			return new Dictionary<string,object>() 
			{
				[_CUSTOMERID] = item.CustomerID,
				[_CUSTOMERTYPEID] = item.CustomerTypeID
			};
		}
		
		public static CustomerCustomerDemo FromDictionary(this IDictionary<string, object> dict)
		{
			return new CustomerCustomerDemo
			{
				CustomerID = (string)dict[_CUSTOMERID],
				CustomerTypeID = (string)dict[_CUSTOMERTYPEID]
			};
		}
		
		public static bool Compare(this CustomerCustomerDemo a, CustomerCustomerDemo b)
		{
			return a.CustomerID == b.CustomerID
			&& a.CustomerTypeID == b.CustomerTypeID;
		}
		
		public static void Copy(this CustomerCustomerDemo from, CustomerCustomerDemo to)
		{
			to.CustomerID = from.CustomerID;
			to.CustomerTypeID = from.CustomerTypeID;
		}
		
		public static CustomerCustomerDemoAssociation GetAssociation(this CustomerCustomerDemo entity, IQuery query)
		{
			return GetAssociation(new CustomerCustomerDemo[] { entity }, query).FirstOrDefault();
		}
		
		public static IEnumerable<CustomerCustomerDemoAssociation> GetAssociation(this IEnumerable<CustomerCustomerDemo> entities, IQuery query)
		{
			var reader = query.Expand(entities);
			
			var associations = new List<CustomerCustomerDemoAssociation>();
			
			var _Customer = reader.Read<Customers>();
			var _CustomerDemographic = reader.Read<CustomerDemographics>();
			
			foreach (var entity in entities)
			{
				var association = new CustomerCustomerDemoAssociation
				{
					Customer = new EntityRef<Customers>(_Customer.FirstOrDefault(row => row.CustomerID == entity.CustomerID)),
					CustomerDemographic = new EntityRef<CustomerDemographics>(_CustomerDemographic.FirstOrDefault(row => row.CustomerTypeID == entity.CustomerTypeID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public static string ToSimpleString(this CustomerCustomerDemo obj)
		{
			return string.Format("{{CustomerID:{0}, CustomerTypeID:{1}}}", 
			obj.CustomerID, 
			obj.CustomerTypeID);
		}
		
		public const string _CUSTOMERID = "CustomerID";
		public const string _CUSTOMERTYPEID = "CustomerTypeID";
	}
}