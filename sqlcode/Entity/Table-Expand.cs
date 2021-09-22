using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data.Text;

namespace Sys.Data.Entity
{
	public sealed partial class Table<TEntity>
	{

		/// <summary>
		/// Expand single assoication immediately
		/// </summary>
		/// <typeparam name="TSubEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public IEnumerable<TSubEntity> Expand<TSubEntity>(TEntity entity) where TSubEntity : class
			=> Expand<TSubEntity>(new TEntity[] { entity });

		/// <summary>
		/// Expand single assoication immediately
		/// </summary>
		/// <typeparam name="TSubEntity"></typeparam>
		/// <param name="entities"></param>
		/// <returns></returns>
		public IEnumerable<TSubEntity> Expand<TSubEntity>(IEnumerable<TEntity> entities) where TSubEntity : class
		{
			IConstraint a = Constraint<TSubEntity>();
			return Expand<TSubEntity>(entities, a.ThisKey, a.OtherKey);
		}

		internal IEnumerable<TSubEntity> Expand<TSubEntity>(IEnumerable<TEntity> entities, string thisKey, string otherKey) where TSubEntity : class
		{
			var where = Contains(entities, thisKey, otherKey).ToScript(Context.Style);
			var table = Context.GetTable<TSubEntity>();
			return table.Select(where);
		}

		/// <summary>
		/// Expand single association
		/// </summary>
		/// <typeparam name="TSubEntity"></typeparam>
		/// <param name="entity"></param>
		public void ExpandOnSubmit<TSubEntity>(TEntity entity) where TSubEntity : class
			=> ExpandOnSubmit<TSubEntity>(new TEntity[] { entity });

		/// <summary>
		/// Expand single association
		/// </summary>
		/// <typeparam name="TSubEntity"></typeparam>
		/// <param name="entities"></param>
		public void ExpandOnSubmit<TSubEntity>(IEnumerable<TEntity> entities) where TSubEntity : class
		{
			IConstraint a = Constraint<TSubEntity>();
			ExpandOnSubmit<TSubEntity>(entities, a.ThisKey, a.OtherKey);
		}

		private void ExpandOnSubmit<TSubEntity>(IEnumerable<TEntity> entities, string thisKey, string otherKey) where TSubEntity : class
		{
			var where = Contains(entities, thisKey, otherKey).ToScript(Context.Style);
			var table = Context.GetTable<TSubEntity>();
			table.SelectOnSubmit(where);
		}

		/// <summary>
		/// Expand all associations
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public Type[] ExpandOnSubmit(TEntity entity)
		{
			return ExpandOnSubmit(new TEntity[] { entity });
		}

		/// <summary>
		/// Expand all associations
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		public Type[] ExpandOnSubmit(IEnumerable<TEntity> entities)
		{
			List<Type> types = new List<Type>();

			foreach (var a in schema.Constraints)
			{
				var schema = broker.GetSchmea(a.OtherType);
				var formalName = schema.FormalTableName();

				Expression where = Contains(entities,a.ThisKey, a.OtherKey);

				var SQL = new SqlBuilder().SELECT().COLUMNS().FROM(formalName).WHERE(where).ToScript(Context.Style);

				Context.CodeBlock.AppendQuery(a.OtherType, SQL);
				types.Add(a.OtherType);
			}

			return types.ToArray();
		}

		private IConstraint Constraint<TSubEntity>()
		{
			IConstraint a = schema.Constraints?.FirstOrDefault(x => x.OtherType == typeof(TSubEntity));
			if (a == null)
				throw new InvalidConstraintException($"invalid assoication from {typeof(TEntity)} to {typeof(TSubEntity)}");
			return a;
		}

		private Expression Contains(IEnumerable<TEntity> entities, string thisKey, string otherKey)
		{
			return otherKey.AsColumn().IN(Values(entities, thisKey));
		}

		public IEnumerable<object> Values(IEnumerable<TEntity> entities, string column)
		{
			return entities.Select(entity => broker.ToDictionary(entity)[column]).ToList();
		}

		private static Expression EQUALS(string column, object value)
		{
			return column.AsColumn() == new Expression(new SqlValue(value));
		}


	}
}
