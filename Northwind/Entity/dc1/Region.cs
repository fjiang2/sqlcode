using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc1
{
	public partial class Region
	{
		public int RegionID { get; set; }
		public string RegionDescription { get; set; }
	}
	
	public class RegionAssociation
	{
		public EntitySet<Territories> Territories { get; set; }
	}
	
	public static class RegionExtension
	{
		public const string TableName = "Region";
		public static readonly string[] Keys = new string[] { _REGIONID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<Territories>
			{
				ThisKey = _REGIONID,
				OtherKey = TerritoriesExtension._REGIONID,
				OneToMany = true
			}
		};
		
		public static List<Region> ToRegionCollection(this DataTable dt)
		{
			return dt.AsEnumerable()
			.Select(row =>
			{
				var obj = new Region();
				FillObject(obj, row);
				return obj;
			})
			.ToList();
		}
		
		public static void FillObject(this Region item, DataRow row)
		{
			item.RegionID = row.GetField<int>(_REGIONID);
			item.RegionDescription = row.GetField<string>(_REGIONDESCRIPTION);
		}
		
		public static void UpdateRow(this Region item, DataRow row)
		{
			row.SetField(_REGIONID, item.RegionID);
			row.SetField(_REGIONDESCRIPTION, item.RegionDescription);
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable { TableName = TableName };
			dt.Columns.Add(new DataColumn(_REGIONID, typeof(int)) { Unique = true, AllowDBNull = false });
			dt.Columns.Add(new DataColumn(_REGIONDESCRIPTION, typeof(string)) { AllowDBNull = false, MaxLength = 50 });
			
			dt.PrimaryKey = dt.Columns.OfType<DataColumn>().Where(column => Keys.Contains(column.ColumnName)).ToArray();
			
			return dt;
		}
		
		public static void ToDataTable(this IEnumerable<Region> items, DataTable dt)
		{
			foreach (var item in items)
			{
				var row = dt.NewRow();
				UpdateRow(item, row);
				dt.Rows.Add(row);
			}
			dt.AcceptChanges();
		}
		
		public static IDictionary<string, object> ToDictionary(this Region item)
		{
			return new Dictionary<string,object>() 
			{
				[_REGIONID] = item.RegionID,
				[_REGIONDESCRIPTION] = item.RegionDescription
			};
		}
		
		public static Region FromDictionary(this IDictionary<string, object> dict)
		{
			return new Region
			{
				RegionID = (int)dict[_REGIONID],
				RegionDescription = (string)dict[_REGIONDESCRIPTION]
			};
		}
		
		public static bool Compare(this Region a, Region b)
		{
			return a.RegionID == b.RegionID
			&& a.RegionDescription == b.RegionDescription;
		}
		
		public static void Copy(this Region from, Region to)
		{
			to.RegionID = from.RegionID;
			to.RegionDescription = from.RegionDescription;
		}
		
		public static RegionAssociation GetAssociation(this Region entity, IQuery query)
		{
			return GetAssociation(new Region[] { entity }, query).FirstOrDefault();
		}
		
		public static IEnumerable<RegionAssociation> GetAssociation(this IEnumerable<Region> entities, IQuery query)
		{
			var reader = query.Expand(entities);
			
			var associations = new List<RegionAssociation>();
			
			var _Territories = reader.Read<Territories>();
			
			foreach (var entity in entities)
			{
				var association = new RegionAssociation
				{
					Territories = new EntitySet<Territories>(_Territories.Where(row => row.RegionID == entity.RegionID)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public static string ToSimpleString(this Region obj)
		{
			return string.Format("{{RegionID:{0}, RegionDescription:{1}}}", 
			obj.RegionID, 
			obj.RegionDescription);
		}
		
		public const string _REGIONID = "RegionID";
		public const string _REGIONDESCRIPTION = "RegionDescription";
	}
}