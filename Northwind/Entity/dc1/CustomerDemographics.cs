using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc1
{
	public partial class CustomerDemographics
	{
		public string CustomerTypeID { get; set; }
		public string CustomerDesc { get; set; }
	}
	
	public class CustomerDemographicsAssociation
	{
		public EntitySet<CustomerCustomerDemo> CustomerCustomerDemoes { get; set; }
	}
	
	public static class CustomerDemographicsExtension
	{
		public const string TableName = "CustomerDemographics";
		public static readonly string[] Keys = new string[] { _CUSTOMERTYPEID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<CustomerCustomerDemo>
			{
				ThisKey = _CUSTOMERTYPEID,
				OtherKey = CustomerCustomerDemoExtension._CUSTOMERTYPEID,
				OneToMany = true
			}
		};
		
		public static List<CustomerDemographics> ToCustomerDemographicsCollection(this DataTable dt)
		{
			return dt.AsEnumerable()
			.Select(row =>
			{
				var obj = new CustomerDemographics();
				FillObject(obj, row);
				return obj;
			})
			.ToList();
		}
		
		public static void FillObject(this CustomerDemographics item, DataRow row)
		{
			item.CustomerTypeID = row.GetField<string>(_CUSTOMERTYPEID);
			item.CustomerDesc = row.GetField<string>(_CUSTOMERDESC);
		}
		
		public static void UpdateRow(this CustomerDemographics item, DataRow row)
		{
			row.SetField(_CUSTOMERTYPEID, item.CustomerTypeID);
			row.SetField(_CUSTOMERDESC, item.CustomerDesc);
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable { TableName = TableName };
			dt.Columns.Add(new DataColumn(_CUSTOMERTYPEID, typeof(string)) { Unique = true, AllowDBNull = false, MaxLength = 10 });
			dt.Columns.Add(new DataColumn(_CUSTOMERDESC, typeof(string)));
			
			dt.PrimaryKey = dt.Columns.OfType<DataColumn>().Where(column => Keys.Contains(column.ColumnName)).ToArray();
			
			return dt;
		}
		
		public static void ToDataTable(this IEnumerable<CustomerDemographics> items, DataTable dt)
		{
			foreach (var item in items)
			{
				var row = dt.NewRow();
				UpdateRow(item, row);
				dt.Rows.Add(row);
			}
			dt.AcceptChanges();
		}
		
		public static IDictionary<string, object> ToDictionary(this CustomerDemographics item)
		{
			return new Dictionary<string,object>() 
			{
				[_CUSTOMERTYPEID] = item.CustomerTypeID,
				[_CUSTOMERDESC] = item.CustomerDesc
			};
		}
		
		public static CustomerDemographics FromDictionary(this IDictionary<string, object> dict)
		{
			return new CustomerDemographics
			{
				CustomerTypeID = (string)dict[_CUSTOMERTYPEID],
				CustomerDesc = (string)dict[_CUSTOMERDESC]
			};
		}
		
		public static bool Compare(this CustomerDemographics a, CustomerDemographics b)
		{
			return a.CustomerTypeID == b.CustomerTypeID
			&& a.CustomerDesc == b.CustomerDesc;
		}
		
		public static void Copy(this CustomerDemographics from, CustomerDemographics to)
		{
			to.CustomerTypeID = from.CustomerTypeID;
			to.CustomerDesc = from.CustomerDesc;
		}
		
		public static CustomerDemographicsAssociation GetAssociation(this CustomerDemographics entity, IQuery query)
		{
			return GetAssociation(new CustomerDemographics[] { entity }, query).FirstOrDefault();
		}
		
		public static IEnumerable<CustomerDemographicsAssociation> GetAssociation(this IEnumerable<CustomerDemographics> entities, IQuery query)
		{
			var reader = query.Expand(entities);
			
			var associations = new List<CustomerDemographicsAssociation>();
			
			var _CustomerCustomerDemoes = reader.Read<CustomerCustomerDemo>();
			
			foreach (var entity in entities)
			{
				var association = new CustomerDemographicsAssociation
				{
					CustomerCustomerDemoes = new EntitySet<CustomerCustomerDemo>(_CustomerCustomerDemoes.Where(row => row.CustomerTypeID == entity.CustomerTypeID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public static string ToSimpleString(this CustomerDemographics obj)
		{
			return string.Format("{{CustomerTypeID:{0}, CustomerDesc:{1}}}", 
			obj.CustomerTypeID, 
			obj.CustomerDesc);
		}
		
		public const string _CUSTOMERTYPEID = "CustomerTypeID";
		public const string _CUSTOMERDESC = "CustomerDesc";
	}
}