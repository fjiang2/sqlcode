using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data;

namespace Sys.Data.Entity
{
    public static class Query
    {
        /// <summary>
        /// It must be assigned before using class Query. The agent could be SQL Server, SQLite, SQLCe agents.
        /// </summary>
        public static IDbAgent DefaultAgent { get; set; }

        private static DataQuery query => new DataQuery(DefaultAgent);

        /// <summary>
        /// Create DbCommand
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static DbAccess DbAccess(SqlUnit unit)
            => new DbAccessDelegate(DefaultAgent, unit);

        /// <summary>
        /// Fill data table
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable FillDataTable(string sql, object args = null)
            => DbAccess(new SqlUnit(sql, args)).FillDataTable();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql, object args = null)
            => DbAccess(new SqlUnit(sql, args)).ExecuteNonQuery();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T Invoke<T>(Func<DataContext, T> func)
        {
            using (var db = new DataContext(DefaultAgent))
            {
                return func(db);
            }
        }

        /// <summary>
        /// Operate a table and submit changes
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static int Submit<TEntity>(Action<Table<TEntity>> action) where TEntity : class
        {
            return Invoke(db =>
            {
                var table = db.GetTable<TEntity>();
                action(table);
                return db.SubmitChanges();
            });
        }

        /// <summary>
        /// Use SelectOnSumbit(...) in action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IQueryResultReader Select(Action<DataContext> action)
            => query.Select(action);

        /// <summary>
        /// SELECT * FROM entity-table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TEntity> Select<TEntity>() where TEntity : class
            => query.Select<TEntity>();

        /// <summary>
        /// Retrieve maxRecord starting on startRecord: SELECT * FROM entity-table WHERE ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="startRecord"> The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> Select<TEntity>(string where, DbLoadOption option) where TEntity : class
            => query.Select<TEntity>(where, option);

        /// <summary>
        /// SELECT * FROM entity-table WHERE ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> Select<TEntity>(string where) where TEntity : class
            => query.Select<TEntity>(where);


        public static IEnumerable<TEntity> Select<TEntity>(Text.Expression where) where TEntity : class
            => query.Select<TEntity>(where);

        /// <summary>
        /// Select single entity by primary key. Properties of primary key must have values
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TEntity SelectRow<TEntity>(TEntity entity) where TEntity : class
            => query.SelectRow(entity);

        /// <summary>
        /// SELECT * FROM entity-table WHERE ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
            => query.Select(where);

        public static IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, bool>> where, DbLoadOption option) where TEntity : class
            => query.Select(where, option);

        /// <summary>
        /// SELECT * FROM entity-table WHERE key-selector IN (SELECT result-selector FROM result-table WHERE ...)
        /// e.g.
        ///   Query.Select<Categories, int, Products>(row => row.CategoryName == "Beverages", row => row.CategoryID, row => row.CategoryID);
        ///   SELECT * FROM [Products] WHERE CategoryID IN (SELECT CategoryID FROM Categories WHERE CategoryName == 'Beverages')
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="where"></param>
        /// <param name="keySelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Select<TEntity, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> keySelector, Expression<Func<TResult, object>> resultSelector)
            where TEntity : class
            where TResult : class
            => query.Select(where, keySelector, resultSelector);


        /// <summary>
        /// INSERT INTO entity-table (...) VALUE (....)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int Insert<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => Submit<TEntity>(table => table.InsertOnSubmit(entities));

        /// <summary>
        /// Insert a single row with entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int InsertRow<TEntity>(TEntity entity) where TEntity : class
            => Submit<TEntity>(table => table.InsertOnSubmit(entity));

        /// <summary>
        /// UPDATE entity-table SET ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int Update<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => Submit<TEntity>(table => table.UpdateOnSubmit(entities));

        /// <summary>
        /// Update a single row with entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int UpdateRow<TEntity>(TEntity entity) where TEntity : class
            => Submit<TEntity>(table => table.UpdateOnSubmit(entity));

        /// <summary>
        /// Update partial columns of entity, values of primary key requried
        /// example of partial entity
        ///   1.object: new { Id=7, Name="XXXX"} 
        ///   2.Dictionary: new Dictionary&lt;string, object&gt;{["Id"]=7, ["Name"]="XXXX"}</string>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static int PartialUpdate<TEntity>(this IEnumerable<object> entities, bool throwException = false) where TEntity : class
            => Submit<TEntity>(table => table.PartialUpdateOnSubmit(entities, throwException));

        /// <summary>
        /// Update a single row with entity partially
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static int PartialUpdateRow<TEntity>(object entity, bool throwException = false) where TEntity : class
            => Submit<TEntity>(table => table.PartialUpdateOnSubmit(entity, throwException));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="modifiedProperties">The properties are modified</param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static int PatialUpdate<TEntity>(TEntity entity, Expression<Func<TEntity, object>> modifiedProperties, Expression<Func<TEntity, bool>> where) where TEntity : class
            => Submit<TEntity>(table => table.PartialUpdateOnSubmit(entity, modifiedProperties, where));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int InsertOrUpdate<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => Submit<TEntity>(table => table.InsertOrUpdateOnSubmit(entities));

        /// <summary>
        /// Insert or update a single row with entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int InsertOrUpdateRow<TEntity>(TEntity entity) where TEntity : class
            => Submit<TEntity>(table => table.InsertOrUpdateOnSubmit(entity));

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int Delete<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => Submit<TEntity>(table => table.DeleteOnSubmit(entities));

        /// <summary>
        /// Delete a single row with entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int DeleteRow<TEntity>(TEntity entity) where TEntity : class
            => query.DeleteRow(entity);

        /// <summary>
        /// DELETE FROM entity-table WHERE ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static int Delete<TEntity>(string where) where TEntity : class
            => query.Delete<TEntity>(where);

        /// <summary>
        /// DELETE FROM entity-table WHERE ...
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static int Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
            => query.Delete<TEntity>(where);

        /// <summary>
        /// Expand detail table from master table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TSubEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static IEnumerable<TSubEntity> Expand<TEntity, TSubEntity>(this IEnumerable<TEntity> entities) where TEntity : class where TSubEntity : class
            => query.Expand<TEntity, TSubEntity>(entities);

        /// <summary>
        /// Expand assocation tables with foreign keys
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static IQueryResultReader Expand<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => query.Expand(entities);


        /// <summary>
        /// Support SqlCe and SQL server, use primary key to check row existence
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int UpsertRow<TEntity>(TEntity entity) where TEntity : class
            => query.UpsertRow(entity);

        /// <summary>
        /// Support SqlCe and SQL server
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static int Upsert<TEntity>(this IEnumerable<TEntity> entities) where TEntity : class
            => query.Upsert(entities);

        /// <summary>
        /// Bulk insert entities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="batchSize"></param>
        public static void BulkInsert<TEntity>(this IEnumerable<TEntity> entities, int batchSize) where TEntity : class
            => query.BulkInsert(entities, batchSize);

     
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="work"></param>
        /// <param name="batchSize"></param>
        public static void Batch<TEntity>(this IEnumerable<TEntity> entities, Action<IEnumerable<TEntity>> work, int batchSize) where TEntity : class
        {
            if (batchSize <= 0)
            {
                work(entities);
            }
            else
            {
                foreach (var list in entities.Split(batchSize))
                {
                    work(list);
                }
            }
        }
    }

}
