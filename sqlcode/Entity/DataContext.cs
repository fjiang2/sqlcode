using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Sys.Data.Entity
{

	public partial class DataContext : IDisposable
	{
		private readonly Dictionary<Type, ITable> tables = new Dictionary<Type, ITable>();
		private readonly DbCmdFunction function;

		internal SqlCodeBlock CodeBlock { get; } = new SqlCodeBlock();
		internal List<RowEvent> RowEvents { get; } = new List<RowEvent>();

		public string Description { get; set; }

		/// <summary>
		///  event triggered when a row inserting, updating or deleting
		/// </summary>
		public event EventHandler<RowEventArgs> RowChanging;

		/// <summary>
		/// event triggered when a row inserted, updated or deleted
		/// </summary>
		public event EventHandler<RowEventArgs> RowChanged;

		/// <summary>
		/// DataContext using extension class (dc1) or single class (dc2)
		/// </summary>
		public static EntityClassType EntityClassType { get; set; } = EntityClassType.ExtensionClass;
		public DbAgentOption Option { get; }

		public DataContext(IDbAgent agent)
		{
			this.function = agent.Function;
			this.Option = agent.Option;
			this.Description = "SQL command handler";
		}

		public void Dispose()
		{
			CodeBlock.Clear();
			tables.Clear();
		}

		protected void OnRowChanging(IEnumerable<RowEvent> evt)
		{
			RowChanging?.Invoke(this, new RowEventArgs(evt));
		}

		protected void OnRowChanged(IEnumerable<RowEvent> evt)
		{
			RowChanged?.Invoke(this, new RowEventArgs(evt));
		}

		public Table<TEntity> GetTable<TEntity>()
			where TEntity : class
		{
			Type key = typeof(TEntity);
			if (tables.ContainsKey(key))
				return (Table<TEntity>)tables[key];

			var obj = new Table<TEntity>(this);
			tables.Add(key, obj);
			return obj;
		}

		public string GetNonQueryScript()
		{
			return string.Join(Environment.NewLine, CodeBlock.GetNonQuery());
		}

		public string GetQueryScript()
		{
			return string.Join(Environment.NewLine, CodeBlock.GetQuery());
		}



		internal DataTable FillDataTable(string query)
		{
			DataSet ds = FillDataSet(new string[] { query });
			if (ds == null)
				return null;

			if (ds.Tables.Count >= 1)
				return ds.Tables[0];

			return null;
		}

		private DataSet FillDataSet(string[] query)
		{
			var parameter = new DbCmdParameter(query, args: null);
			var cmd = function(parameter);
			var ds = new DataSet();
			cmd.FillDataSet(ds);
			return ds;
		}

		public IQueryResultReader SumbitQueries()
		{
			if (CodeBlock.Length == 0)
				return null;

			string[] query = CodeBlock.GetQuery();
			Type[] types = CodeBlock.GetQueryTypes();
			var ds = FillDataSet(query);
			CodeBlock.Clear();

			return new QueryResultReader(this, types, ds);
		}

		public int SubmitChanges()
		{
			if (CodeBlock.Length == 0)
				return -1;

			OnRowChanging(RowEvents);

			var parameter = new DbCmdParameter(CodeBlock.GetNonQuery(), args: null);
			var cmd = function(parameter);
			int count = cmd.ExecuteNonQuery();
			CodeBlock.Clear();

			OnRowChanged(RowEvents);
			RowEvents.Clear();

			return count;
		}


		public override string ToString()
		{
			return Description;
		}

	}
}
