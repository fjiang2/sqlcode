using Sys.Data.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Sys.Data.Entity
{
    public interface IDbQuery : IDisposable
    {
        DbAccess Access(SqlBuilder sql, object args);
        DbAccess Access(SqlUnit unit);
        DbAccess Access(string query);
        DbAccess Access(string query, object args);
        
        void BulkInsert<TEntity>(IEnumerable<TEntity> entities, int batchSize) where TEntity : class;
        
        int Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        int Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        int Delete<TEntity>(string where) where TEntity : class;
        int DeleteRow<TEntity>(TEntity entity) where TEntity : class;

        IEnumerable<TSubEntity> Expand<TEntity, TSubEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
            where TSubEntity : class;
        IQueryResultReader Expand<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        int Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        int InsertOrUpdate<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        int InsertOrUpdateRow<TEntity>(TEntity entity) where TEntity : class;
        int InsertRow<TEntity>(TEntity entity) where TEntity : class;

        int PartialUpdate<TEntity>(IEnumerable<object> entities, bool throwException = false) where TEntity : class;
        int PartialUpdate<TEntity>(TEntity entity, Expression<Func<TEntity, object>> modifiedProperties, Expression<Func<TEntity, bool>> where) where TEntity : class;
        int PartialUpdateRow<TEntity>(object entity, bool throwException = false) where TEntity : class;

        IQueryResultReader Select(Action<DbContext> action);
        IEnumerable<TResult> Select<TEntity, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> keySelector, Expression<Func<TResult, object>> resultSelector)
            where TEntity : class
            where TResult : class;
        IEnumerable<TEntity> Select<TEntity>() where TEntity : class;
        IEnumerable<TEntity> Select<TEntity>(Text.Expression where) where TEntity : class;
        IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, bool>> where, DbLoadOption option) where TEntity : class;
        IEnumerable<TEntity> Select<TEntity>(string where) where TEntity : class;
        IEnumerable<TEntity> Select<TEntity>(string where, DbLoadOption option) where TEntity : class;
        TEntity SelectRow<TEntity>(TEntity entity) where TEntity : class;
        
        int Submit<TEntity>(Action<Table<TEntity>> action) where TEntity : class;
        List<TEntity> ToList<TEntity>(DataTable dataTable) where TEntity : class;
        
        int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        int UpdateRow<TEntity>(TEntity entity) where TEntity : class;
        int Upsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        int UpsertRow<TEntity>(TEntity entity) where TEntity : class;
    }
}