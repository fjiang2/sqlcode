using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc1
{
	public partial class EmployeeTerritories
	{
		public int EmployeeID { get; set; }
		public string TerritoryID { get; set; }
	}
	
	public class EmployeeTerritoriesAssociation
	{
		public EntityRef<Employees> Employee { get; set; }
		public EntityRef<Territories> Territory { get; set; }
	}
	
	public static class EmployeeTerritoriesExtension
	{
		public const string TableName = "EmployeeTerritories";
		public static readonly string[] Keys = new string[] { _EMPLOYEEID, _TERRITORYID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Employees>
			{
				Name = "FK_EmployeeTerritories_Employees",
				ThisKey = _EMPLOYEEID,
				OtherKey = EmployeesExtension._EMPLOYEEID,
				IsForeignKey = true
			},
			new Constraint<Territories>
			{
				Name = "FK_EmployeeTerritories_Territories",
				ThisKey = _TERRITORYID,
				OtherKey = TerritoriesExtension._TERRITORYID,
				IsForeignKey = true
			}
		};
		
		public static List<EmployeeTerritories> ToEmployeeTerritoriesCollection(this DataTable dt)
		{
			return dt.AsEnumerable()
			.Select(row =>
			{
				var obj = new EmployeeTerritories();
				FillObject(obj, row);
				return obj;
			})
			.ToList();
		}
		
		public static void FillObject(this EmployeeTerritories item, DataRow row)
		{
			item.EmployeeID = row.GetField<int>(_EMPLOYEEID);
			item.TerritoryID = row.GetField<string>(_TERRITORYID);
		}
		
		public static void UpdateRow(this EmployeeTerritories item, DataRow row)
		{
			row.SetField(_EMPLOYEEID, item.EmployeeID);
			row.SetField(_TERRITORYID, item.TerritoryID);
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable { TableName = TableName };
			dt.Columns.Add(new DataColumn(_EMPLOYEEID, typeof(int)) { AllowDBNull = false });
			dt.Columns.Add(new DataColumn(_TERRITORYID, typeof(string)) { AllowDBNull = false, MaxLength = 20 });
			
			dt.PrimaryKey = dt.Columns.OfType<DataColumn>().Where(column => Keys.Contains(column.ColumnName)).ToArray();
			
			return dt;
		}
		
		public static void ToDataTable(this IEnumerable<EmployeeTerritories> items, DataTable dt)
		{
			foreach (var item in items)
			{
				var row = dt.NewRow();
				UpdateRow(item, row);
				dt.Rows.Add(row);
			}
			dt.AcceptChanges();
		}
		
		public static IDictionary<string, object> ToDictionary(this EmployeeTerritories item)
		{
			return new Dictionary<string,object>() 
			{
				[_EMPLOYEEID] = item.EmployeeID,
				[_TERRITORYID] = item.TerritoryID
			};
		}
		
		public static EmployeeTerritories FromDictionary(this IDictionary<string, object> dict)
		{
			return new EmployeeTerritories
			{
				EmployeeID = (int)dict[_EMPLOYEEID],
				TerritoryID = (string)dict[_TERRITORYID]
			};
		}
		
		public static bool Compare(this EmployeeTerritories a, EmployeeTerritories b)
		{
			return a.EmployeeID == b.EmployeeID
			&& a.TerritoryID == b.TerritoryID;
		}
		
		public static void Copy(this EmployeeTerritories from, EmployeeTerritories to)
		{
			to.EmployeeID = from.EmployeeID;
			to.TerritoryID = from.TerritoryID;
		}
		
		public static EmployeeTerritoriesAssociation GetAssociation(this EmployeeTerritories entity, IQuery query)
		{
			return GetAssociation(new EmployeeTerritories[] { entity }, query).FirstOrDefault();
		}
		
		public static IEnumerable<EmployeeTerritoriesAssociation> GetAssociation(this IEnumerable<EmployeeTerritories> entities, IQuery query)
		{
			var reader = query.Expand(entities);
			
			var associations = new List<EmployeeTerritoriesAssociation>();
			
			var _Employee = reader.Read<Employees>();
			var _Territory = reader.Read<Territories>();
			
			foreach (var entity in entities)
			{
				var association = new EmployeeTerritoriesAssociation
				{
					Employee = new EntityRef<Employees>(_Employee.FirstOrDefault(row => row.EmployeeID == entity.EmployeeID)),
					Territory = new EntityRef<Territories>(_Territory.FirstOrDefault(row => row.TerritoryID == entity.TerritoryID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public static string ToSimpleString(this EmployeeTerritories obj)
		{
			return string.Format("{{EmployeeID:{0}, TerritoryID:{1}}}", 
			obj.EmployeeID, 
			obj.TerritoryID);
		}
		
		public const string _EMPLOYEEID = "EmployeeID";
		public const string _TERRITORYID = "TerritoryID";
	}
}