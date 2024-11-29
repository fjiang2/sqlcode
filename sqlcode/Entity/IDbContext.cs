using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sys.Data.Entity
{
    public interface IDbContext
    {
        string Description { get; set; }
        DbAgentOption Option { get; }
        DbAgentStyle Style { get; }

        event EventHandler<RowEventArgs> RowChanged;
        event EventHandler<RowEventArgs> RowChanging;

        void Clear();
        void Dispose();

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