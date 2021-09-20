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
		{
			string where = AssociationWhere<TSubEntity>(entity).ToScript(Context.Style);

			var table = Context.GetTable<TSubEntity>();
			return table.Select(where);
		}

		/// <summary>
		/// Expand single assoication immediately
		/// </summary>
		/// <typeparam name="TSubEntity"></typeparam>
		/// <param name="entities"></param>
		/// <returns></returns>
		public IEnumerable<TSubEntity> Expand<TSubEntity>(IEnumerable<TEntity> entities) where TSubEntity : class
		{
			string where = AssociationWhere<TSubEntity>(entities).ToScript(Context.Style);

			var table = Context.GetTable<TSubEntity>();
			return table.Select(where);
		}


		/// <summary>
		/// Expand single association
		/// </summary>
		/// <typeparam name="TSubEntity"></typeparam>
		/// <param name="entity"></param>
		public void ExpandOnSubmit<TSubEntity>(TEntity entity) where TSubEntity : class
		{
			string where = AssociationWhere<TSubEntity>(entity).ToScript(Context.Style);

			var table = Context.GetTable<TSubEntity>();
			table.SelectOnSubmit(where);
		}

		/// <summary>
		/// Expand single association
		/// </summary>
		/// <typeparam name="TSubEntity"></typeparam>
		/// <param name="entities"></param>
		public void ExpandOnSubmit<TSubEntity>(IEnumerable<TEntity> entities) where TSubEntity : class
		{
			string where = AssociationWhere<TSubEntity>(entities).ToScript(Context.Style);

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
			List<Type> types = new List<Type>();
			var dict = broker.ToDictionary(entity);

			foreach (var a in schema.Constraints)
			{
				var schema = broker.GetSchmea(a.OtherType);
				var formalName = schema.FormalTableName();

				object value = dict[a.ThisKey];
				Expression where = Compare(a.OtherKey, value);
				var SQL = new SqlBuilder().SELECT().COLUMNS().FROM(formalName).WHERE(where).ToScript(Context.Style);

				Context.CodeBlock.AppendQuery(a.OtherType, SQL);
				types.Add(a.OtherType);
			}

			return types.ToArray();
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

				Expression where = Compare(a.OtherKey, entities.Select(entity => broker.ToDictionary(entity)[a.ThisKey]));
				var SQL = new SqlBuilder().SELECT().COLUMNS().FROM(formalName).WHERE(where).ToScript(Context.Style);

				Context.CodeBlock.AppendQuery(a.OtherType, SQL);
				types.Add(a.OtherType);
			}

			return types.ToArray();
		}

		private Expression AssociationWhere<TSubEntity>(TEntity entity)
		{
			IConstraint a = schema.Constraints?.FirstOrDefault(x => x.OtherType == typeof(TSubEntity));
			if (a == null)
				throw new InvalidConstraintException($"invalid assoication from {typeof(TEntity)} to {typeof(TSubEntity)}");

			var dict = broker.ToDictionary(entity);
			object value = dict[a.ThisKey];
			return Compare(a.OtherKey, value);
		}

		private Expression AssociationWhere<TSubEntity>(IEnumerable<TEntity> entities)
		{
			IConstraint a = schema.Constraints?.FirstOrDefault(x => x.OtherType == typeof(TSubEntity));
			if (a == null)
				throw new InvalidConstraintException($"invalid assoication from {typeof(TEntity)} to {typeof(TSubEntity)}");

			return Compare(a.OtherKey, entities.Select(entity => broker.ToDictionary(entity)[a.ThisKey]));
		}

		private static Expression Compare(string column, object value)
		{
			return column.AsColumn() == new Expression(new SqlValue(value));
		}

		private static Expression Compare(string column, IEnumerable<object> values)
		{
			return column.AsColumn().IN(values);
		}
	}
}
