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
		private readonly IDbAgent agent;

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

		public DataContext(IDbAgent agent)
		{
			this.agent = agent;
			this.Description = nameof(DataContext);
		}

		public void Dispose()
		{
			CodeBlock.Clear();
			tables.Clear();
		}

		public DbAgentOption Option => agent.Option;
		public DbAgentStyle Style => agent.Option.Style;

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

		/// <summary>
		/// It can be used for Bulk copy/insert
		/// </summary>
		/// <returns>key is table-name, value is an array of INSERT statements</returns>
		public IDictionary<Type, string[]> GetBulkInsert() => CodeBlock.GetBulkInsert();

		internal DataTable FillDataTable(string query, int startRecord, int maxRecords)
		{
			var unit = new SqlUnit(query);
			var cmd = agent.Proxy(unit);
			var dt = new DataTable();
			cmd.FillDataTable(dt, startRecord, maxRecords);
			return dt;
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
			var unit = new SqlUnit(query);
			var cmd = agent.Proxy(unit);
			var ds = new DataSet();
			cmd.FillDataSet(ds);
			return ds;
		}

		public IQueryResultReader SumbitQueries()
		{
			if (CodeBlock.Length == 0)
				return null;

			try
			{
				string[] query = CodeBlock.GetQuery();
				Type[] types = CodeBlock.GetQueryTypes();
				var ds = FillDataSet(query);
				return new QueryResultReader(this, types, ds);
			}
			finally
			{
				CodeBlock.Clear();
			}
		}

		public int SubmitChanges()
		{
			if (CodeBlock.Length == 0)
				return -1;

			try
			{
				OnRowChanging(RowEvents);
				
				var unit = new SqlUnit(CodeBlock.GetNonQuery());
				var cmd = agent.Proxy(unit);
				int count = cmd.ExecuteNonQuery();
				
				OnRowChanged(RowEvents);
				return count;
			}
			finally
			{
				CodeBlock.Clear();
				RowEvents.Clear();
			}
		}

		/// <summary>
		/// Clear generated code and row events
		/// </summary>
		public void Clear()
		{
			CodeBlock.Clear();
			RowEvents.Clear();
		}

		public override string ToString()
		{
			return Description;
		}

	}
}
