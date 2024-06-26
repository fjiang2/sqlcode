using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc1
{
	public partial class Territories
	{
		public string TerritoryID { get; set; }
		public string TerritoryDescription { get; set; }
		public int RegionID { get; set; }
	}
	
	public class TerritoriesAssociation
	{
		public EntitySet<EmployeeTerritories> EmployeeTerritories { get; set; }
		public EntityRef<Region> Region { get; set; }
	}
	
	public static class TerritoriesExtension
	{
		public const string TableName = "Territories";
		public static readonly string[] Keys = new string[] { _TERRITORYID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<EmployeeTerritories>
			{
				ThisKey = _TERRITORYID,
				OtherKey = EmployeeTerritoriesExtension._TERRITORYID,
				OneToMany = true
			},
			new Constraint<Region>
			{
				Name = "FK_Territories_Region",
				ThisKey = _REGIONID,
				OtherKey = RegionExtension._REGIONID,
				IsForeignKey = true
			}
		};
		
		public static List<Territories> ToTerritoriesCollection(this DataTable dt)
		{
			return dt.AsEnumerable()
			.Select(row =>
			{
				var obj = new Territories();
				FillObject(obj, row);
				return obj;
			})
			.ToList();
		}
		
		public static void FillObject(this Territories item, DataRow row)
		{
			item.TerritoryID = row.GetField<string>(_TERRITORYID);
			item.TerritoryDescription = row.GetField<string>(_TERRITORYDESCRIPTION);
			item.RegionID = row.GetField<int>(_REGIONID);
		}
		
		public static void UpdateRow(this Territories item, DataRow row)
		{
			row.SetField(_TERRITORYID, item.TerritoryID);
			row.SetField(_TERRITORYDESCRIPTION, item.TerritoryDescription);
			row.SetField(_REGIONID, item.RegionID);
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable { TableName = TableName };
			dt.Columns.Add(new DataColumn(_TERRITORYID, typeof(string)) { Unique = true, AllowDBNull = false, MaxLength = 20 });
			dt.Columns.Add(new DataColumn(_TERRITORYDESCRIPTION, typeof(string)) { AllowDBNull = false, MaxLength = 50 });
			dt.Columns.Add(new DataColumn(_REGIONID, typeof(int)) { AllowDBNull = false });
			
			dt.PrimaryKey = dt.Columns.OfType<DataColumn>().Where(column => Keys.Contains(column.ColumnName)).ToArray();
			
			return dt;
		}
		
		public static void ToDataTable(this IEnumerable<Territories> items, DataTable dt)
		{
			foreach (var item in items)
			{
				var row = dt.NewRow();
				UpdateRow(item, row);
				dt.Rows.Add(row);
			}
			dt.AcceptChanges();
		}
		
		public static IDictionary<string, object> ToDictionary(this Territories item)
		{
			return new Dictionary<string,object>() 
			{
				[_TERRITORYID] = item.TerritoryID,
				[_TERRITORYDESCRIPTION] = item.TerritoryDescription,
				[_REGIONID] = item.RegionID
			};
		}
		
		public static Territories FromDictionary(this IDictionary<string, object> dict)
		{
			return new Territories
			{
				TerritoryID = (string)dict[_TERRITORYID],
				TerritoryDescription = (string)dict[_TERRITORYDESCRIPTION],
				RegionID = (int)dict[_REGIONID]
			};
		}
		
		public static bool Compare(this Territories a, Territories b)
		{
			return a.TerritoryID == b.TerritoryID
			&& a.TerritoryDescription == b.TerritoryDescription
			&& a.RegionID == b.RegionID;
		}
		
		public static void Copy(this Territories from, Territories to)
		{
			to.TerritoryID = from.TerritoryID;
			to.TerritoryDescription = from.TerritoryDescription;
			to.RegionID = from.RegionID;
		}
		
		public static TerritoriesAssociation GetAssociation(this Territories entity, IQuery query)
		{
			return GetAssociation(new Territories[] { entity }, query).FirstOrDefault();
		}
		
		public static IEnumerable<TerritoriesAssociation> GetAssociation(this IEnumerable<Territories> entities, IQuery query)
		{
			var reader = query.Expand(entities);
			
			var associations = new List<TerritoriesAssociation>();
			
			var _EmployeeTerritories = reader.Read<EmployeeTerritories>();
			var _Region = reader.Read<Region>();
			
			foreach (var entity in entities)
			{
				var association = new TerritoriesAssociation
				{
					EmployeeTerritories = new EntitySet<EmployeeTerritories>(_EmployeeTerritories.Where(row => row.TerritoryID == entity.TerritoryID)),
					Region = new EntityRef<Region>(_Region.FirstOrDefault(row => row.RegionID == entity.RegionID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public static string ToSimpleString(this Territories obj)
		{
			return string.Format("{{TerritoryID:{0}, TerritoryDescription:{1}, RegionID:{2}}}", 
			obj.TerritoryID, 
			obj.TerritoryDescription, 
			obj.RegionID);
		}
		
		public const string _TERRITORYID = "TerritoryID";
		public const string _TERRITORYDESCRIPTION = "TerritoryDescription";
		public const string _REGIONID = "RegionID";
	}
}