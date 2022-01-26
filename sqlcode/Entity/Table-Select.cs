using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Sys.Data.Entity
{
    public sealed partial class Table<TEntity>
    {

        public IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> where, DbLoadMode mode = DbLoadMode.DbFill)
        {
            var translator = new QueryTranslator(Context.Style);
            string _where = translator.Translate(where);
            return Select(_where, mode);
        }

        public IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> where, DbLoadOption option)
        {
            var translator = new QueryTranslator(Context.Style);
            string _where = translator.Translate(where);
            return Select(_where, option);
        }

        public IEnumerable<TEntity> Select(string where, DbLoadMode mode = DbLoadMode.DbFill)
        {
            string SQL = SelectFromWhere(where);

            var dt = Context.LoadDataTable(SQL, mode);
            return ToList(dt);
        }

        public IEnumerable<TEntity> Select(string where, DbLoadOption option)
        {
            string SQL = SelectFromWhere(where);
            var dt = Context.LoadDataTable(SQL, option);
            return ToList(dt);
        }


        /// <summary>
        /// Read single entity by primary key
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public TEntity Select(TEntity where)
        {
            SqlGenerator gen = this.Generator;
            Type type = typeof(TEntity);

            foreach (string key in schema.PrimaryKeys)
            {
                object obj = type.GetProperty(key)?.GetValue(where);
                gen.Add(key, obj);
            }

            string SQL = gen.SelectRows();
            gen.Clear();

            var dt = Context.LoadDataTable(SQL, DbLoadMode.DbFill);
            return ToList(dt).FirstOrDefault();
        }

        public void SelectOnSubmit(Expression<Func<TEntity, bool>> where)
        {
            var translator = new QueryTranslator(Context.Style);
            string _where = translator.Translate(where);
            SelectOnSubmit(_where);
        }

        public void SelectOnSubmit(string where)
        {
            string SQL = SelectFromWhere(where);
            Context.CodeBlock.AppendQuery<TEntity>(SQL);
        }


        public List<TEntity> ToList(DataTable dt)
        {
            return broker.ToList(dt);
        }

        private string SelectFromWhere(string where)
        {
            return SelectFromWhere(where, null);
        }

        internal string SelectFromWhere(string where, IEnumerable<string> columns)
        {
            return new Text.SqlBuilder().SELECT().COLUMNS(columns).FROM(formalName).WHERE(where).ToScript(Context.Style);
        }

    }
}
