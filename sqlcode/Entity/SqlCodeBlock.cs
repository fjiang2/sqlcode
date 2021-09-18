using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sys.Data.Entity
{
	class SqlCodeBlock
	{
		class SqlStatement
		{
			public Type Table { get; set; }
			public string Statement { get; set; }
			public bool NonQuery { get; set; }
			public RowOperation Operation { get; set; }
			public override string ToString() => Statement;
		}

		private readonly List<SqlStatement> clauses = new List<SqlStatement>();

		public void AppendLine<TEntity>(RowOperation operation, string clause)
		{
			AppendLine(typeof(TEntity), operation, clause);
		}

		public void AppendLine(Type table, RowOperation operation, string clause)
		{
			clauses.Add(new SqlStatement
			{
				Table = table,
				Statement = clause,
				Operation = operation,
				NonQuery = true,
			});
		}

		public void AppendQuery<TEntity>(string clause)
		{
			AppendQuery(typeof(TEntity), clause);
		}

		public void AppendQuery(Type table, string clause)
		{
			clauses.Add(new SqlStatement
			{
				Table = table,
				Statement = clause,
				Operation = RowOperation.Select,
				NonQuery = false,
			});
		}

		public int Length => clauses.Count;

		public void Clear() => clauses.Clear();

		public string[] GetNonQuery()
		{
			var L = clauses.Where(x => x.NonQuery).Select(x => x.Statement);
			return L.ToArray();
		}

		public IDictionary<Type, string[]> GetBulkInsert()
		{
			return clauses
				.Where(x => x.NonQuery && x.Operation == RowOperation.Insert)
				.GroupBy(x => x.Table)
				.ToDictionary(g => g.Key, g => g.Select(x => x.Statement).ToArray());
		}

		public string[] GetQuery()
		{
			var L = clauses.Where(x => !x.NonQuery).Select(x => x.Statement);
			return L.ToArray();
		}

		public Type[] GetQueryTypes()
		{
			return clauses.Where(x => !x.NonQuery).Select(x => x.Table).ToArray();
		}

		public override string ToString()
		{
			return string.Join(Environment.NewLine, clauses.Select(x => x.Statement));
		}
	}
}
