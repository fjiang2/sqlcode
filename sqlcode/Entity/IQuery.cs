using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sys.Data.Entity
{
	public interface IQuery
	{
		IEnumerable<TResult> Select<TEntity, TKey, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TResult, TKey>> resultSelector) where TEntity : class where TResult : class;
		IEnumerable<TEntity> Select<TEntity>() where TEntity : class;
		IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
		IEnumerable<TEntity> Select<TEntity>(string where = null) where TEntity : class;

		IEnumerable<TSubEntity> Expand<TEntity, TSubEntity>(IEnumerable<TEntity> entities) where TEntity : class where TSubEntity : class;

		int Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		int InsertOrUpdate<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		int Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
		int Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		int Delete<TEntity>(string where = null) where TEntity : class;

		IQueryResultReader Expand<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		IQueryResultReader Select(Action<DataContext> action);

	}
}