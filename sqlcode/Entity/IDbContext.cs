using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sys.Data.Entity
{
    public interface IDbContext : IDisposable
    {
        string Description { get; set; }
        DbAgentOption Option { get; }
        DbAgentStyle Style { get; }

        event EventHandler<RowEventArgs> RowChanged;
        event EventHandler<RowEventArgs> RowChanging;

        /// <summary>
        /// Clear generated code and row events
        /// </summary>
        void Clear();

        IEnumerable<TSubEntity> Expand<TEntity, TSubEntity>(IEnumerable<TEntity> entities) 
            where TEntity : class 
            where TSubEntity : class;
        IEnumerable<TSubEntity> Expand<TEntity, TSubEntity>(TEntity entity) 
            where TEntity : class 
            where TSubEntity : class;
        IQueryResultReader Expand<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        IQueryResultReader Expand<TEntity>(TEntity entity) where TEntity : class;
        
        void ExpandOnSubmit<TEntity, TSubEntity>(TEntity entity) 
            where TEntity : class 
            where TSubEntity : class;
        Type[] ExpandOnSubmit<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        Type[] ExpandOnSubmit<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// It can be used for Bulk copy/insert
        /// </summary>
        /// <returns>key is table-name, value is an array of INSERT statements</returns>
        IDictionary<Type, string[]> GetBulkInsert();
        string GetNonQueryScript();
        string GetQueryScript();

        Table<TEntity> GetTable<TEntity>() where TEntity : class;
        
        IEnumerable<TEntity> Select<TEntity>(Text.Expression where) where TEntity : class;
        IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        void SelectOnSubmit<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;

        int SubmitChanges();
        IQueryResultReader SubmitQueries();
    }
}