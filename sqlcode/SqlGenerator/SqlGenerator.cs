using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

namespace Sys.Data
{
	/// <summary>
	/// Generate SQL CRUD script
	/// </summary>
	public class SqlGenerator : SqlColumnValuePairCollection
	{
		/// <summary>
		/// Table name
		/// </summary>
		public string TableName { get; }

		/// <summary>
		/// A list of primary keys
		/// </summary>
		public string[] PrimaryKeys { get; set; }

		/// <summary>
		/// a list of identity column names
		/// </summary>
		public string[] IdentityKeys { get; set; }

		/// <summary>
		/// Search condition, 
		/// Use primary-keys as search condition if this property is not empty.
		/// </summary>
		public string Where { get; set; } = string.Empty;

		private readonly SqlTemplate template;

		public SqlGenerator(string formalName)
		{
			this.TableName = formalName;
			this.template = new SqlTemplate(formalName);
		}

		public override SqlColumnValuePair Add(string name, object value)
		{
			var pair = base.Add(name, value);

			pair.Column.Primary = PrimaryKeys != null && PrimaryKeys.Contains(name);
			pair.Column.Identity = IdentityKeys != null && IdentityKeys.Contains(name);

			return pair;
		}

		private string[] notUpdateColumns => columns.Where(p => !p.Column.Saved).Select(p => p.Column.Name).ToArray();

		/// <summary>
		/// SELECT * FROM Table WHERE condition
		/// </summary>
		/// <returns></returns>
		public string Select()
		{
			string where = Condition();
			if (string.IsNullOrEmpty(where))
				return template.Select("*");

			return template.Select("*", where);
		}

		/// <summary>
		/// SELECT * FROM Table
		/// </summary>
		/// <returns></returns>
		public string SelectRows() => SelectRows("*");

		/// <summary>
		/// SELECT Col1,Col2,.. FROM Table
		/// </summary>
		/// <param name="columns"></param>
		/// <returns></returns>
		public string SelectRows(IEnumerable<string> columns)
		{
			var L1 = string.Join(",", columns.Select(c => SqlColumnValuePair.FormalName(c)));
			if (L1 == string.Empty)
				L1 = "*";

			return SelectRows(L1);
		}

		private string SelectRows(string columns) => template.Select(columns);

		/// <summary>
		/// INSERT... or UPDATE...
		/// </summary>
		/// <param name="exists"></param>
		/// <returns></returns>
		public string InsertOrUpdate(bool? exists)
		{
			if (exists == false)
				return Insert();

			if (exists == true)
				return Update();

			return InsertOrUpdate();
		}

		/// <summary>
		/// IF NOT EXISTS INSERT... ELSE UPDATE ...
		/// </summary>
		/// <returns></returns>
		public string InsertOrUpdate()
		{
			string where = Condition();
			if (string.IsNullOrEmpty(where))
			{
				throw new InvalidOperationException("WHERE is blank");
			}

			if (PrimaryKeys.Length + notUpdateColumns.Length == columns.Count)
			{
				return template.IfNotExistsInsert(where, Insert());
			}
			else
			{
				return template.IfExistsUpdateElseInsert(where, Update(), Insert());
			}
		}

		/// <summary>
		///  IF NOT EXISTS INSERT...
		/// </summary>
		/// <returns></returns>
		public string InsertIfNotExists()
		{
			string where = Condition();
			if (string.IsNullOrEmpty(where))
			{
				throw new InvalidOperationException("WHERE is blank");
			}

			return template.IfNotExistsInsert(where, Insert());
		}

		/// <summary>
		/// INSERT INTO table (Col1,Col2,...) VALUES (val1,val2,...)
		/// </summary>
		/// <returns></returns>
		public string Insert()
		{
			var C = columns.Where(c => !c.Column.Identity && !c.Value.IsNull);
			var L1 = string.Join(",", C.Select(c => c.ColumnFormalName));
			var L2 = string.Join(",", C.Select(c => c.Value.ToString()));

			return template.Insert(L1, L2);
		}

		/// <summary>
		/// Insert record and return identity value which is supported by SQL Server
		/// INSERT INTO table (Col1,Col2,...) VALUES (val1,val2,...); SET @col=@@IDENTITY
		/// </summary>
		/// <returns></returns>
		public string InsertWithIdentityParameter()
		{
			var pair = columns.FirstOrDefault(c => c.Column.Identity);

			string insert = Insert();
			if (pair != null)
			{
				var parameterName = new Text.ParameterName(pair.Column.Name);
				string clause = SqlTemplate.SetIdentityOutParameter(parameterName.ToString());
				return $"{insert}; {clause}";
			}

			return insert;
		}

		/// <summary>
		/// IF EXISTS ... UPDATE ...
		/// </summary>
		/// <returns></returns>
		public string UpdateIfExists()
		{
			string where = Condition();
			if (string.IsNullOrEmpty(where))
			{
				throw new InvalidOperationException("WHERE is blank");
			}

			return template.IfExistsUpdate(where, Update());
		}

		/// <summary>
		/// UPDATE Table SET ... WHERE ...
		/// </summary>
		/// <returns></returns>
		public string Update()
		{
			var C2 = columns.Where(c => !PrimaryKeys.Contains(c.ColumnName) && !notUpdateColumns.Contains(c.ColumnName));
			var L2 = string.Join(",", C2.Select(c => c.ToString()));

			if (C2.Count() == 0)
				return string.Empty;

			string where = Condition();

			if (string.IsNullOrEmpty(where))
				return template.Update(L2);

			return template.Update(L2, where);
		}

		/// <summary>
		/// DELETE FROM Table WHERE ...
		/// </summary>
		/// <returns></returns>
		public string Delete()
		{
			string where = Condition();

			if (string.IsNullOrEmpty(where))
				return template.Delete();

			return template.Delete(where);
		}

		/// <summary>
		/// DELETE FROM Table
		/// </summary>
		/// <returns></returns>
		public string DeleteAll()
		{
			return template.Delete();
		}

		private string Condition()
		{
			if (!string.IsNullOrEmpty(Where))
				return Where;

			if (PrimaryKeys.Length > 0)
			{
				var C1 = columns.Where(c => PrimaryKeys.Contains(c.ColumnName));
				var L1 = string.Join(" AND ", C1.Select(c => c.ToString()));
				return L1;
			}

			return string.Empty;
		}
	}
}
