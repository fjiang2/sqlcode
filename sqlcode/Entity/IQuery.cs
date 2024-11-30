using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sys.Data.Entity
{
	public interface IQuery
	{
		IEnumerable<TEntity> Select<TEntity>() where TEntity : class;
		IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
		IEnumerable<TEntity> Select<TEntity>(string where) where TEntity : class;

		int Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		int InsertOrUpdate<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

		int Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
		int Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		int Delete<TEntity>(string where) where TEntity : class;

		IQueryResultReader Expand<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		IQueryResultReader Select(Action<DbContext> action);

	}
}