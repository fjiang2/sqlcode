using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc1
{
	public partial class Shippers
	{
		public int ShipperID { get; set; }
		public string CompanyName { get; set; }
		public string Phone { get; set; }
	}
	
	public class ShippersAssociation
	{
		public EntitySet<Orders> Orders { get; set; }
	}
	
	public static class ShippersExtension
	{
		public const string TableName = "Shippers";
		public static readonly string[] Keys = new string[] { _SHIPPERID };
		public static readonly string[] Identity = new string[] { _SHIPPERID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Orders>
			{
				ThisKey = _SHIPPERID,
				OtherKey = OrdersExtension._SHIPVIA,
				OneToMany = true
			}
		};
		
		public static List<Shippers> ToShippersCollection(this DataTable dt)
		{
			return dt.AsEnumerable()
			.Select(row =>
			{
				var obj = new Shippers();
				FillObject(obj, row);
				return obj;
			})
			.ToList();
		}
		
		public static void FillObject(this Shippers item, DataRow row)
		{
			item.ShipperID = row.GetField<int>(_SHIPPERID);
			item.CompanyName = row.GetField<string>(_COMPANYNAME);
			item.Phone = row.GetField<string>(_PHONE);
		}
		
		public static void UpdateRow(this Shippers item, DataRow row)
		{
			row.SetField(_SHIPPERID, item.ShipperID);
			row.SetField(_COMPANYNAME, item.CompanyName);
			row.SetField(_PHONE, item.Phone);
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable { TableName = TableName };
			dt.Columns.Add(new DataColumn(_SHIPPERID, typeof(int)) { Unique = true, AllowDBNull = false, AutoIncrement = true });
			dt.Columns.Add(new DataColumn(_COMPANYNAME, typeof(string)) { AllowDBNull = false, MaxLength = 40 });
			dt.Columns.Add(new DataColumn(_PHONE, typeof(string)) { MaxLength = 24 });
			
			dt.PrimaryKey = dt.Columns.OfType<DataColumn>().Where(column => Keys.Contains(column.ColumnName)).ToArray();
			
			return dt;
		}
		
		public static void ToDataTable(this IEnumerable<Shippers> items, DataTable dt)
		{
			foreach (var item in items)
			{
				var row = dt.NewRow();
				UpdateRow(item, row);
				dt.Rows.Add(row);
			}
			dt.AcceptChanges();
		}
		
		public static IDictionary<string, object> ToDictionary(this Shippers item)
		{
			return new Dictionary<string,object>() 
			{
				[_SHIPPERID] = item.ShipperID,
				[_COMPANYNAME] = item.CompanyName,
				[_PHONE] = item.Phone
			};
		}
		
		public static Shippers FromDictionary(this IDictionary<string, object> dict)
		{
			return new Shippers
			{
				ShipperID = (int)dict[_SHIPPERID],
				CompanyName = (string)dict[_COMPANYNAME],
				Phone = (string)dict[_PHONE]
			};
		}
		
		public static bool Compare(this Shippers a, Shippers b)
		{
			return a.ShipperID == b.ShipperID
			&& a.CompanyName == b.CompanyName
			&& a.Phone == b.Phone;
		}
		
		public static void Copy(this Shippers from, Shippers to)
		{
			to.ShipperID = from.ShipperID;
			to.CompanyName = from.CompanyName;
			to.Phone = from.Phone;
		}
		
		public static ShippersAssociation GetAssociation(this Shippers entity, IQuery query)
		{
			return GetAssociation(new Shippers[] { entity }, query).FirstOrDefault();
		}
		
		public static IEnumerable<ShippersAssociation> GetAssociation(this IEnumerable<Shippers> entities, IQuery query)
		{
			var reader = query.Expand(entities);
			
			var associations = new List<ShippersAssociation>();
			
			var _Orders = reader.Read<Orders>();
			
			foreach (var entity in entities)
			{
				var association = new ShippersAssociation
				{
					Orders = new EntitySet<Orders>(_Orders.Where(row => row.ShipVia == entity.ShipperID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public static string ToSimpleString(this Shippers obj)
		{
			return string.Format("{{ShipperID:{0}, CompanyName:{1}, Phone:{2}}}", 
			obj.ShipperID, 
			obj.CompanyName, 
			obj.Phone);
		}
		
		public const string _SHIPPERID = "ShipperID";
		public const string _COMPANYNAME = "CompanyName";
		public const string _PHONE = "Phone";
	}
}