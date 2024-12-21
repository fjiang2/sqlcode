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

        /// <summary>
        ///  Expand association tables with foreign keys
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        IQueryResultReader Expand<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        int Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        int InsertOrUpdate<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        int InsertOrUpdateRow<TEntity>(TEntity entity) where TEntity : class;
        int InsertRow<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Update partial columns of entity, values of primary key required
        /// example of partial entity
        ///   1.object: new { Id=7, Name="XXXX"} 
        ///   2.Dictionary: new Dictionary&lt;string, object&gt;{["Id"]=7, ["Name"]="XXXX"}</string>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        int PartialUpdate<TEntity>(IEnumerable<object> entities, bool throwException = false) where TEntity : class;

        /// <summary>
        /// Update entity partially
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="modifiedProperties">The properties are modified</param>
        /// <param name="where"></param>
        /// <returns></returns>
        int PartialUpdate<TEntity>(TEntity entity, Expression<Func<TEntity, object>> modifiedProperties, Expression<Func<TEntity, bool>> where) where TEntity : class;

        /// <summary>
        /// Update a single row with entity partially
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        int PartialUpdateRow<TEntity>(object entity, bool throwException = false) where TEntity : class;

        IQueryResultReader Select(Action<DbContext> action);

        /// <summary>
        /// SELECT * FROM entity-table WHERE key-selector IN (SELECT result-selector FROM result-table WHERE ...)
        /// e.g.
        ///   Query.Select<Categories, int, Products>(row => row.CategoryName == "Beverages", row => row.CategoryID, row => row.CategoryID);
        ///   SELECT * FROM [Products] WHERE CategoryID IN (SELECT CategoryID FROM Categories WHERE CategoryName == 'Beverages')
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="where"></param>
        /// <param name="keySelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        IEnumerable<TResult> Select<TEntity, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> keySelector, Expression<Func<TResult, object>> resultSelector)
            where TEntity : class
            where TResult : class;

        /// <summary>
        /// Get all rows of a table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IEnumerable<TEntity> Select<TEntity>() where TEntity : class;

        /// <summary>
        /// Use SqlBuilder to select rows
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Support SqlCe and SQL server
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        int Upsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        /// <summary>
        /// Support SqlCe and SQL server, use primary key to check row existence
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        int UpsertRow<TEntity>(TEntity entity) where TEntity : class;
    }
}