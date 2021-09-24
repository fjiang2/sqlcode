using System.Collections.Generic;

namespace Sys.Data.Entity
{
    /// <summary>
    /// Query result reader
    /// </summary>
    public interface IQueryResultReader
    {
        /// <summary>
        /// Read the result from insert-on-submit tables
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IEnumerable<TEntity> Read<TEntity>() where TEntity : class;
    }
}