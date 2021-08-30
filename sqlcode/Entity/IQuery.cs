using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sys.Data.Entity
{
	public interface IQuery
	{
		int Delete<TEntity>(string where = null) where TEntity : class;
		int Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
		int Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		IEnumerable<TSubEntity> Expand<TEntity, TSubEntity>(IEnumerable<TEntity> entities) where TEntity : class where TSubEntity : class;
		IQueryResultReader Expand<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		int Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		int InsertOrUpdate<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		BaseDbCmd NewDbCmd(string sql, object args);
		int PatialUpdate<TEntity>(IEnumerable<object> entities, bool throwException = false) where TEntity : class;
		int PatialUpdate<TEntity>(TEntity entity, Expression<Func<TEntity, object>> modifiedProperties, Expression<Func<TEntity, bool>> where) where TEntity : class;
		IQueryResultReader Select(Action<DataContext> action);
		IEnumerable<TResult> Select<TEntity, TKey, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TResult, TKey>> resultSelector) where TEntity : class where TResult : class;
		IEnumerable<TEntity> Select<TEntity>() where TEntity : class;
		IEnumerable<TEntity> Select<TEntity>(string where = null) where TEntity : class;
		IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
		IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, object>> selectedColumns, Expression<Func<TEntity, bool>> where = null) where TEntity : class, new();
		int Submit<TEntity>(Action<Table<TEntity>> action) where TEntity : class;
		int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
	}
}