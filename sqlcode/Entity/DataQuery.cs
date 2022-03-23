using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data;
using Sys.Data.Text;

namespace Sys.Data.Entity
{
	public class DataQuery : IQuery
	{
		private readonly IDbAgent agent;

		public DataQuery(IDbAgent agent)
		{
			this.agent = agent ?? throw new ArgumentNullException("undefined agent");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		public DbAccess Access(SqlUnit unit)
		{
			return new DbAccessDelegate(agent, unit);
		}

		public DbAccess Access(SqlBuilder sql, object args) => Access(new SqlUnit(sql.ToScript(agent.Option.Style), args));
		
		public DbAccess Access(string query) => Access(new SqlUnit(query));

		public DbAccess Access(string query, object args) => Access(new SqlUnit(query, args));

		private T Invoke<T>(Func<DataContext, T> func)
		{
			using (var db = new DataContext(agent))
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
		public int Submit<TEntity>(Action<Table<TEntity>> action) where TEntity : class
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
		public IQueryResultReader Select(Action<DataContext> action)
		{
			return Invoke(db =>
			{
				action(db);
				return db.SumbitQueries();
			});
		}


		/// <summary>
		/// SELECT * FROM entity-table
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <returns></returns>
		public IEnumerable<TEntity> Select<TEntity>() where TEntity : class
		{
			return Invoke(db => db.GetTable<TEntity>().Select(where: string.Empty));
		}

		public IEnumerable<TEntity> Select<TEntity>(string where, DbLoadOption option) where TEntity : class
		{
			return Invoke(db => db.GetTable<TEntity>().Select(where, option));
		}

		/// <summary>
		/// SELECT * FROM entity-table WHERE ...
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="where"></param>
		/// <returns></returns>
		public IEnumerable<TEntity> Select<TEntity>(string where) where TEntity : class
		{
			return Invoke(db => db.GetTable<TEntity>().Select(where));
		}

		public IEnumerable<TEntity> Select<TEntity>(Text.Expression where) where TEntity : class
		{
			return Invoke(db => db.Select<TEntity>(where));
		}

		/// <summary>
		/// Select single entity by primary key. Properties of primary key must have values
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public TEntity SelectRow<TEntity>(TEntity entity) where TEntity : class
		{
			return Invoke(db =>
			{
				var table = db.GetTable<TEntity>();
				return table.Select(entity);
			});
		}

		/// <summary>
		/// SELECT * FROM entity-table WHERE ...
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="where"></param>
		/// <returns></returns>
		public IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
		{
			return Invoke(db => db.GetTable<TEntity>().Select(where));
		}

		public IEnumerable<TEntity> Select<TEntity>(Expression<Func<TEntity, bool>> where, DbLoadOption option) where TEntity : class
		{
			return Invoke(db => db.GetTable<TEntity>().Select(where, option));
		}

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
		public IEnumerable<TResult> Select<TEntity, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> keySelector, Expression<Func<TResult, object>> resultSelector)
			where TEntity : class
			where TResult : class
		{
			var translator = new QueryTranslator(agent.Option.Style);
			string _where = translator.Translate(where);
			string _keySelector = translator.Translate(keySelector);
			string _resultSelector = translator.Translate(resultSelector);

			return Invoke(db =>
			{
				var dt = db.GetTable<TEntity>();
				string SQL = _resultSelector
					.AsColumn()
					.IN(new SqlBuilder().SELECT().COLUMNS(_keySelector).FROM(dt.ToString()).WHERE(_where))
					.ToScript(db.Style);

				return db.GetTable<TResult>().Select(SQL);
			});
		}



		/// <summary>
		/// INSERT INTO entity-table (...) VALUE (....)
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entities"></param>
		/// <returns></returns>
		public int Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
			=> Submit<TEntity>(table => table.InsertOnSubmit(entities));

		/// <summary>
		/// Insert a single row with entity
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public int InsertRow<TEntity>(TEntity entity) where TEntity : class
			=> Submit<TEntity>(table => table.InsertOnSubmit(entity));

		/// <summary>
		/// UPDATE entity-table SET ...
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entities"></param>
		/// <returns></returns>
		public int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
			=> Submit<TEntity>(table => table.UpdateOnSubmit(entities));

		/// <summary>
		/// Update a single row with entity
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public int UpdateRow<TEntity>(TEntity entity) where TEntity : class
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
		public int PartialUpdate<TEntity>(IEnumerable<object> entities, bool throwException = false) where TEntity : class
			=> Submit<TEntity>(table => table.PartialUpdateOnSubmit(entities, throwException));

		/// <summary>
		/// Update a single row with entity partially
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <param name="throwException"></param>
		/// <returns></returns>
		public int PartialUpdateRow<TEntity>(object entity, bool throwException = false) where TEntity : class
			=> Submit<TEntity>(table => table.PartialUpdateOnSubmit(entity, throwException));

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <param name="modifiedProperties">The properties are modified</param>
		/// <param name="where"></param>
		/// <returns></returns>
		public int PatialUpdate<TEntity>(TEntity entity, Expression<Func<TEntity, object>> modifiedProperties, Expression<Func<TEntity, bool>> where) where TEntity : class
			=> Submit<TEntity>(table => table.PartialUpdateOnSubmit(entity, modifiedProperties, where));

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entities"></param>
		/// <returns></returns>
		public int InsertOrUpdate<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
			=> Submit<TEntity>(table => table.InsertOrUpdateOnSubmit(entities));

		/// <summary>
		/// Insert or update a single row with entity
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public int InsertOrUpdateRow<TEntity>(TEntity entity) where TEntity : class
			=> Submit<TEntity>(table => table.InsertOrUpdateOnSubmit(entity));

		/// <summary>
		/// Delete entities
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entities"></param>
		/// <returns></returns>
		public int Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
			=> Submit<TEntity>(table => table.DeleteOnSubmit(entities));

		/// <summary>
		/// Delete a single row with entity
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public int DeleteRow<TEntity>(TEntity entity) where TEntity : class
			=> Submit<TEntity>(table => table.DeleteOnSubmit(entity));

		/// <summary>
		/// DELETE FROM entity-table WHERE ...
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="where"></param>
		/// <returns></returns>
		public int Delete<TEntity>(string where) where TEntity : class
		{
			return Submit<TEntity>(table => table.DeleteOnSubmit(where));
		}

		/// <summary>
		/// DELETE FROM entity-table WHERE ...
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="where"></param>
		/// <returns></returns>
		public int Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
		{
			return Submit<TEntity>(table => table.DeleteOnSubmit(where));
		}

		/// <summary>
		/// Expand detail table from master table
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TSubEntity"></typeparam>
		/// <param name="entities"></param>
		/// <returns></returns>
		public IEnumerable<TSubEntity> Expand<TEntity, TSubEntity>(IEnumerable<TEntity> entities) where TEntity : class where TSubEntity : class
			=> Invoke(db => db.Expand<TEntity, TSubEntity>(entities));

		/// <summary>
		/// Expand assocation tables with foreign keys
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entities"></param>
		/// <returns></returns>
		public IQueryResultReader Expand<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
			=> Invoke(db => db.Expand(entities));


		/// <summary>
		/// Support SqlCe and SQL server, use primary key to check row existence
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public int UpsertRow<TEntity>(TEntity entity) where TEntity : class
		{
			return Submit<TEntity>(table =>
			{
				if (table.Select(entity) == null)
					table.InsertOnSubmit(entity);
				else
					table.UpdateOnSubmit(entity);
			});
		}

		/// <summary>
		/// Support SqlCe and SQL server
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entities"></param>
		/// <returns></returns>
		public int Upsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			return Submit<TEntity>(table =>
			{
				foreach (var entity in entities)
				{
					if (table.Select(entity) == null)
						table.InsertOnSubmit(entity);
					else
						table.UpdateOnSubmit(entity);
				}
			});
		}

		public List<TEntity> ToList<TEntity>(DataTable dataTable) where TEntity : class
		{
			return Invoke(db =>
			{
				var dt = db.GetTable<TEntity>();
				return dt.ToList(dataTable);
			});
		}

		/// <summary>
		/// Bulk insert entities
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entities"></param>
		/// <param name="batchSize"></param>
		public void BulkInsert<TEntity>(IEnumerable<TEntity> entities, int batchSize) where TEntity : class
		{
			Invoke(db =>
			{
				var dt = db.GetTable<TEntity>();
				foreach (var entity in entities)
				{
					dt.InsertOnSubmit(entity);
				}

				var dict = db.GetBulkInsert();
				foreach (var kvp in dict)
				{
					foreach (var list in kvp.Value.Split(batchSize))
					{
						var unit = new SqlUnit(list.ToArray());
						var cmd = agent.Proxy(unit);
						cmd.ExecuteTransaction();
					}
				}

				return 0;
			});
		}

	}

}
