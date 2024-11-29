using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Sys.Data.Entity
{

    /// <summary>
    /// Database context
    /// </summary>
    public partial class DataContext : IDisposable, IDbContext
    {
        private readonly Dictionary<Type, ITable> tables = new Dictionary<Type, ITable>();
        protected readonly IDbAgent agent;

        internal SqlCodeBlock CodeBlock { get; } = new SqlCodeBlock();
        internal List<RowEvent> RowEvents { get; } = new List<RowEvent>();

        /// <summary>
        /// The description of data context
        /// </summary>
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
        public static EntityClassType EntityClassType { get; set; } = EntityClassType.SingleClass;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agent"></param>
        public DataContext(IDbAgent agent)
        {
            this.agent = agent;
            this.Description = nameof(DataContext);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            CodeBlock.Clear();
            tables.Clear();
        }

        /// <summary>
        /// Database agent option
        /// </summary>
        public DbAgentOption Option => agent.Option;

        /// <summary>
        /// Database engine/server type
        /// </summary>
        public DbAgentStyle Style => agent.Option.Style;

        /// <summary>
        /// Event trigger before commit
        /// </summary>
        /// <param name="evt"></param>
        protected void OnRowChanging(IEnumerable<RowEvent> evt)
        {
            RowChanging?.Invoke(this, new RowEventArgs(evt));
        }

        /// <summary>
        /// Event trigger after commit
        /// </summary>
        /// <param name="evt"></param>
        protected void OnRowChanged(IEnumerable<RowEvent> evt)
        {
            RowChanged?.Invoke(this, new RowEventArgs(evt));
        }

        /// <summary>
        /// Get the table instance
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Get all non-query SQL statements 
        /// </summary>
        /// <returns></returns>
        public string GetNonQueryScript()
        {
            return string.Join(Environment.NewLine, CodeBlock.GetNonQuery());
        }

        /// <summary>
        /// Get all query SQL statements
        /// </summary>
        /// <returns></returns>

        public string GetQueryScript()
        {
            return string.Join(Environment.NewLine, CodeBlock.GetQuery());
        }

        /// <summary>
        /// It can be used for Bulk copy/insert
        /// </summary>
        /// <returns>key is table-name, value is an array of INSERT statements</returns>
        public IDictionary<Type, string[]> GetBulkInsert() => CodeBlock.GetBulkInsert();

        internal DataTable LoadDataTable(string query, DbLoadOption option)
        {
            var unit = new SqlUnit(query);
            var cmd = agent.Access(unit);
            var dt = new DataTable();
            if (option.Mode == DbLoadMode.DbFill)
                cmd.FillDataTable(dt, option.StartRecord, option.MaxRecords);
            else
                cmd.ReadDataTable(dt, option.StartRecord, option.MaxRecords);
            return dt;
        }

        internal DataTable LoadDataTable(string query, DbLoadMode mode)
        {
            DataSet ds = LoadDataSet(new string[] { query }, mode);
            if (ds == null)
                return null;

            if (ds.Tables.Count >= 1)
                return ds.Tables[0];

            return null;
        }

        private DataSet LoadDataSet(string[] query, DbLoadMode mode)
        {
            var unit = new SqlUnit(query);
            var cmd = agent.Access(unit);
            var ds = new DataSet();
            if (mode == DbLoadMode.DbFill)
                cmd.FillDataSet(ds);
            else
                cmd.ReadDataSet(ds);
            return ds;
        }

        /// <summary>
        /// Submit query's changes
        /// </summary>
        /// <returns></returns>
        public IQueryResultReader SubmitQueries()
        {
            if (CodeBlock.Length == 0)
                return null;

            try
            {
                string[] query = CodeBlock.GetQuery();
                Type[] types = CodeBlock.GetQueryTypes();
                var ds = LoadDataSet(query, DbLoadMode.DbFill);
                return new QueryResultReader(this, types, ds);
            }
            finally
            {
                CodeBlock.Clear();
            }
        }


        /// <summary>
        /// Submit non-query's changes
        /// </summary>
        /// <returns></returns>
        public int SubmitChanges()
        {
            if (CodeBlock.Length == 0)
                return -1;

            try
            {
                OnRowChanging(RowEvents);

                var unit = new SqlUnit(CodeBlock.GetNonQuery());
                var cmd = agent.Access(unit);
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }

    }
}
