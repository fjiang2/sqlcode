using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Entity;

namespace UnitTestProject.Northwind.dc1
{
	public partial class Employees
	{
		public int EmployeeID { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string Title { get; set; }
		public string TitleOfCourtesy { get; set; }
		public DateTime BirthDate { get; set; }
		public DateTime HireDate { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }
		public string HomePhone { get; set; }
		public string Extension { get; set; }
		public byte[] Photo { get; set; }
		public string Notes { get; set; }
		public int ReportsTo { get; set; }
		public string PhotoPath { get; set; }
	}
	
	public class EmployeesAssociation
	{
		public EntitySet<EmployeeTerritories> EmployeeTerritories { get; set; }
		public EntitySet<Orders> Orders { get; set; }
		public EntityRef<Employees> Employee { get; set; }
	}
	
	public static class EmployeesExtension
	{
		public const string TableName = "Employees";
		public static readonly string[] Keys = new string[] { _EMPLOYEEID };
		public static readonly string[] Identity = new string[] { _EMPLOYEEID };
		
		public static readonly IConstraint[] Constraints = new IConstraint[]
		{
			new Constraint<EmployeeTerritories>
			{
				ThisKey = _EMPLOYEEID,
				OtherKey = EmployeeTerritoriesExtension._EMPLOYEEID,
				OneToMany = true
			},
			new Constraint<Orders>
			{
				ThisKey = _EMPLOYEEID,
				OtherKey = OrdersExtension._EMPLOYEEID,
				OneToMany = true
			},
			new Constraint<Employees>
			{
				Name = "FK_Employees_Employees",
				ThisKey = _REPORTSTO,
				OtherKey = EmployeesExtension._EMPLOYEEID,
				IsForeignKey = true
			}
		};
		
		public static List<Employees> ToEmployeesCollection(this DataTable dt)
		{
			return dt.AsEnumerable()
			.Select(row =>
			{
				var obj = new Employees();
				FillObject(obj, row);
				return obj;
			})
			.ToList();
		}
		
		public static void FillObject(this Employees item, DataRow row)
		{
			item.EmployeeID = row.GetField<int>(_EMPLOYEEID);
			item.LastName = row.GetField<string>(_LASTNAME);
			item.FirstName = row.GetField<string>(_FIRSTNAME);
			item.Title = row.GetField<string>(_TITLE);
			item.TitleOfCourtesy = row.GetField<string>(_TITLEOFCOURTESY);
			item.BirthDate = row.GetField<DateTime>(_BIRTHDATE);
			item.HireDate = row.GetField<DateTime>(_HIREDATE);
			item.Address = row.GetField<string>(_ADDRESS);
			item.City = row.GetField<string>(_CITY);
			item.Region = row.GetField<string>(_REGION);
			item.PostalCode = row.GetField<string>(_POSTALCODE);
			item.Country = row.GetField<string>(_COUNTRY);
			item.HomePhone = row.GetField<string>(_HOMEPHONE);
			item.Extension = row.GetField<string>(_EXTENSION);
			item.Photo = row.GetField<byte[]>(_PHOTO);
			item.Notes = row.GetField<string>(_NOTES);
			item.ReportsTo = row.GetField<int>(_REPORTSTO);
			item.PhotoPath = row.GetField<string>(_PHOTOPATH);
		}
		
		public static void UpdateRow(this Employees item, DataRow row)
		{
			row.SetField(_EMPLOYEEID, item.EmployeeID);
			row.SetField(_LASTNAME, item.LastName);
			row.SetField(_FIRSTNAME, item.FirstName);
			row.SetField(_TITLE, item.Title);
			row.SetField(_TITLEOFCOURTESY, item.TitleOfCourtesy);
			row.SetField(_BIRTHDATE, item.BirthDate);
			row.SetField(_HIREDATE, item.HireDate);
			row.SetField(_ADDRESS, item.Address);
			row.SetField(_CITY, item.City);
			row.SetField(_REGION, item.Region);
			row.SetField(_POSTALCODE, item.PostalCode);
			row.SetField(_COUNTRY, item.Country);
			row.SetField(_HOMEPHONE, item.HomePhone);
			row.SetField(_EXTENSION, item.Extension);
			row.SetField(_PHOTO, item.Photo);
			row.SetField(_NOTES, item.Notes);
			row.SetField(_REPORTSTO, item.ReportsTo);
			row.SetField(_PHOTOPATH, item.PhotoPath);
		}
		
		public static DataTable CreateTable()
		{
			DataTable dt = new DataTable { TableName = TableName };
			dt.Columns.Add(new DataColumn(_EMPLOYEEID, typeof(int)) { Unique = true, AllowDBNull = false, AutoIncrement = true });
			dt.Columns.Add(new DataColumn(_LASTNAME, typeof(string)) { AllowDBNull = false, MaxLength = 20 });
			dt.Columns.Add(new DataColumn(_FIRSTNAME, typeof(string)) { AllowDBNull = false, MaxLength = 10 });
			dt.Columns.Add(new DataColumn(_TITLE, typeof(string)) { MaxLength = 30 });
			dt.Columns.Add(new DataColumn(_TITLEOFCOURTESY, typeof(string)) { MaxLength = 25 });
			dt.Columns.Add(new DataColumn(_BIRTHDATE, typeof(DateTime)));
			dt.Columns.Add(new DataColumn(_HIREDATE, typeof(DateTime)));
			dt.Columns.Add(new DataColumn(_ADDRESS, typeof(string)) { MaxLength = 60 });
			dt.Columns.Add(new DataColumn(_CITY, typeof(string)) { MaxLength = 15 });
			dt.Columns.Add(new DataColumn(_REGION, typeof(string)) { MaxLength = 15 });
			dt.Columns.Add(new DataColumn(_POSTALCODE, typeof(string)) { MaxLength = 10 });
			dt.Columns.Add(new DataColumn(_COUNTRY, typeof(string)) { MaxLength = 15 });
			dt.Columns.Add(new DataColumn(_HOMEPHONE, typeof(string)) { MaxLength = 24 });
			dt.Columns.Add(new DataColumn(_EXTENSION, typeof(string)) { MaxLength = 4 });
			dt.Columns.Add(new DataColumn(_PHOTO, typeof(byte[])));
			dt.Columns.Add(new DataColumn(_NOTES, typeof(string)));
			dt.Columns.Add(new DataColumn(_REPORTSTO, typeof(int)));
			dt.Columns.Add(new DataColumn(_PHOTOPATH, typeof(string)) { MaxLength = 255 });
			
			dt.PrimaryKey = dt.Columns.OfType<DataColumn>().Where(column => Keys.Contains(column.ColumnName)).ToArray();
			
			return dt;
		}
		
		public static void ToDataTable(this IEnumerable<Employees> items, DataTable dt)
		{
			foreach (var item in items)
			{
				var row = dt.NewRow();
				UpdateRow(item, row);
				dt.Rows.Add(row);
			}
			dt.AcceptChanges();
		}
		
		public static IDictionary<string, object> ToDictionary(this Employees item)
		{
			return new Dictionary<string,object>() 
			{
				[_EMPLOYEEID] = item.EmployeeID,
				[_LASTNAME] = item.LastName,
				[_FIRSTNAME] = item.FirstName,
				[_TITLE] = item.Title,
				[_TITLEOFCOURTESY] = item.TitleOfCourtesy,
				[_BIRTHDATE] = item.BirthDate,
				[_HIREDATE] = item.HireDate,
				[_ADDRESS] = item.Address,
				[_CITY] = item.City,
				[_REGION] = item.Region,
				[_POSTALCODE] = item.PostalCode,
				[_COUNTRY] = item.Country,
				[_HOMEPHONE] = item.HomePhone,
				[_EXTENSION] = item.Extension,
				[_PHOTO] = item.Photo,
				[_NOTES] = item.Notes,
				[_REPORTSTO] = item.ReportsTo,
				[_PHOTOPATH] = item.PhotoPath
			};
		}
		
		public static Employees FromDictionary(this IDictionary<string, object> dict)
		{
			return new Employees
			{
				EmployeeID = (int)dict[_EMPLOYEEID],
				LastName = (string)dict[_LASTNAME],
				FirstName = (string)dict[_FIRSTNAME],
				Title = (string)dict[_TITLE],
				TitleOfCourtesy = (string)dict[_TITLEOFCOURTESY],
				BirthDate = (DateTime)dict[_BIRTHDATE],
				HireDate = (DateTime)dict[_HIREDATE],
				Address = (string)dict[_ADDRESS],
				City = (string)dict[_CITY],
				Region = (string)dict[_REGION],
				PostalCode = (string)dict[_POSTALCODE],
				Country = (string)dict[_COUNTRY],
				HomePhone = (string)dict[_HOMEPHONE],
				Extension = (string)dict[_EXTENSION],
				Photo = (byte[])dict[_PHOTO],
				Notes = (string)dict[_NOTES],
				ReportsTo = (int)dict[_REPORTSTO],
				PhotoPath = (string)dict[_PHOTOPATH]
			};
		}
		
		public static bool Compare(this Employees a, Employees b)
		{
			return a.EmployeeID == b.EmployeeID
			&& a.LastName == b.LastName
			&& a.FirstName == b.FirstName
			&& a.Title == b.Title
			&& a.TitleOfCourtesy == b.TitleOfCourtesy
			&& a.BirthDate == b.BirthDate
			&& a.HireDate == b.HireDate
			&& a.Address == b.Address
			&& a.City == b.City
			&& a.Region == b.Region
			&& a.PostalCode == b.PostalCode
			&& a.Country == b.Country
			&& a.HomePhone == b.HomePhone
			&& a.Extension == b.Extension
			&& a.Photo == b.Photo
			&& a.Notes == b.Notes
			&& a.ReportsTo == b.ReportsTo
			&& a.PhotoPath == b.PhotoPath;
		}
		
		public static void Copy(this Employees from, Employees to)
		{
			to.EmployeeID = from.EmployeeID;
			to.LastName = from.LastName;
			to.FirstName = from.FirstName;
			to.Title = from.Title;
			to.TitleOfCourtesy = from.TitleOfCourtesy;
			to.BirthDate = from.BirthDate;
			to.HireDate = from.HireDate;
			to.Address = from.Address;
			to.City = from.City;
			to.Region = from.Region;
			to.PostalCode = from.PostalCode;
			to.Country = from.Country;
			to.HomePhone = from.HomePhone;
			to.Extension = from.Extension;
			to.Photo = from.Photo;
			to.Notes = from.Notes;
			to.ReportsTo = from.ReportsTo;
			to.PhotoPath = from.PhotoPath;
		}
		
		public static EmployeesAssociation GetAssociation(this Employees entity, IQuery query)
		{
			return GetAssociation(new Employees[] { entity }, query).FirstOrDefault();
		}
		
		public static IEnumerable<EmployeesAssociation> GetAssociation(this IEnumerable<Employees> entities, IQuery query)
		{
			var reader = query.Expand(entities);
			
			var associations = new List<EmployeesAssociation>();
			
			var _EmployeeTerritories = reader.Read<EmployeeTerritories>();
			var _Orders = reader.Read<Orders>();
			var _Employee = reader.Read<Employees>();
			
			foreach (var entity in entities)
			{
				var association = new EmployeesAssociation
				{
					EmployeeTerritories = new EntitySet<EmployeeTerritories>(_EmployeeTerritories.Where(row => row.EmployeeID == entity.EmployeeID)),
					Orders = new EntitySet<Orders>(_Orders.Where(row => row.EmployeeID == entity.EmployeeID)),
					Employee = new EntityRef<Employees>(_Employee.FirstOrDefault(row => row.EmployeeID == entity.ReportsTo)),
				};
				associations.Add(association);
			}
			
			return associations;
		}
		
		public static string ToSimpleString(this Employees obj)
		{
			return string.Format("{{EmployeeID:{0}, LastName:{1}, FirstName:{2}, Title:{3}, TitleOfCourtesy:{4}, BirthDate:{5}, HireDate:{6}, Address:{7}, City:{8}, Region:{9}, PostalCode:{10}, Country:{11}, HomePhone:{12}, Extension:{13}, Photo:{14}, Notes:{15}, ReportsTo:{16}, PhotoPath:{17}}}", 
			obj.EmployeeID, 
			obj.LastName, 
			obj.FirstName, 
			obj.Title, 
			obj.TitleOfCourtesy, 
			obj.BirthDate, 
			obj.HireDate, 
			obj.Address, 
			obj.City, 
			obj.Region, 
			obj.PostalCode, 
			obj.Country, 
			obj.HomePhone, 
			obj.Extension, 
			obj.Photo, 
			obj.Notes, 
			obj.ReportsTo, 
			obj.PhotoPath);
		}
		
		public const string _EMPLOYEEID = "EmployeeID";
		public const string _LASTNAME = "LastName";
		public const string _FIRSTNAME = "FirstName";
		public const string _TITLE = "Title";
		public const string _TITLEOFCOURTESY = "TitleOfCourtesy";
		public const string _BIRTHDATE = "BirthDate";
		public const string _HIREDATE = "HireDate";
		public const string _ADDRESS = "Address";
		public const string _CITY = "City";
		public const string _REGION = "Region";
		public const string _POSTALCODE = "PostalCode";
		public const string _COUNTRY = "Country";
		public const string _HOMEPHONE = "HomePhone";
		public const string _EXTENSION = "Extension";
		public const string _PHOTO = "Photo";
		public const string _NOTES = "Notes";
		public const string _REPORTSTO = "ReportsTo";
		public const string _PHOTOPATH = "PhotoPath";
	}
}