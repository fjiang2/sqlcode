﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Sys.Data.Text;

namespace Sys.Data.Entity
{

    public sealed partial class Table<TEntity> : ITable
    {
        private readonly IDataContractBroker<TEntity> broker;
        private readonly string formalName;
        private readonly ITableSchema schema;

        public SqlGenerator Generator { get; }
        public DataContext Context { get; }

        internal Table(DataContext context)
        {
            this.Context = context;
            this.broker = BrokerOfDataContract<TEntity>.CreateBroker(DataContext.EntityClassType);
            this.schema = broker.Schema;
            this.formalName = schema.FormalTableName();

            this.Generator = SqlGenerator.Create(formalName, context.Option);
            this.Generator.PrimaryKeys = schema.PrimaryKeys;
            this.Generator.IdentityKeys = schema.IdentityKeys;
        }



        public void InsertOnSubmit(TEntity entity) => OperateOnSubmit(RowOperation.Insert, entity);
        public void UpdateOnSubmit(TEntity entity) => OperateOnSubmit(RowOperation.Update, entity);
        public void InsertOrUpdateOnSubmit(TEntity entity) => OperateOnSubmit(RowOperation.InsertOrUpdate, entity);
        public void DeleteOnSubmit(TEntity entity) => OperateOnSubmit(RowOperation.Delete, entity);

        public void InsertOnSubmit(IEnumerable<TEntity> entities) => OperateOnSubmitRange(RowOperation.Insert, entities);
        public void UpdateOnSubmit(IEnumerable<TEntity> entities) => OperateOnSubmitRange(RowOperation.Update, entities);
        public void InsertOrUpdateOnSubmit(IEnumerable<TEntity> entities) => OperateOnSubmitRange(RowOperation.InsertOrUpdate, entities);
        public void DeleteOnSubmit(IEnumerable<TEntity> entities) => OperateOnSubmitRange(RowOperation.Delete, entities);

        public void DeleteOnSubmit(Expression<Func<TEntity, bool>> where)
        {
            var translator = new QueryTranslator(Context.Style);
            string _where = translator.Translate(where);
            DeleteOnSubmit(_where);
        }

        public void DeleteOnSubmit(string where)
        {
            string SQL = new SqlBuilder().DELETE_FROM(formalName).WHERE(where).ToScript(Context.Style);
            Context.CodeBlock.AppendLine<TEntity>(RowOperation.Delete, SQL);
        }

        public void PartialUpdateOnSubmit(IEnumerable<object> entities, bool throwException = false)
        {
            foreach (var entity in entities)
            {
                PartialUpdateOnSubmit(entity, throwException);
            }
        }

        private void OperateOnSubmitRange(RowOperation operation, IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                OperateOnSubmit(operation, entity);
            }
        }

        private void OperateOnSubmit(RowOperation operation, TEntity entity)
        {
            SqlGenerator gen = this.Generator;

            var dict = broker.ToDictionary(entity);
            gen.AddRange(dict);

            string sql = null;
            switch (operation)
            {
                case RowOperation.Insert:
                    sql = gen.Insert();
                    break;

                case RowOperation.Update:
                    sql = gen.Update();
                    break;

                case RowOperation.InsertOrUpdate:
                    sql = gen.InsertOrUpdate();
                    break;

                case RowOperation.Delete:
                    sql = gen.Delete();
                    break;
            }

            if (string.IsNullOrEmpty(sql))
                return;

            Append(sql, operation, gen.ToDictionary());
            gen.Clear();
        }



        /// <summary>
        /// Update partial columns of entity, values of primary key requried
        /// </summary>
        /// <param name="entity">
        /// example of partial entity
        /// 1.object: new { Id=7, Name="XXXX"} 
        /// 2.Dictionary: new Dictionary&lt;string, object&gt;{["Id"]=7, ["Name"]="XXXX"}</string>
        /// </param>
        /// <param name="throwException">check column existence</param>
        public void PartialUpdateOnSubmit(object entity, bool throwException = false)
        {
            if (entity == null)
            {
                if (throwException)
                    throw new ArgumentNullException($"argument {nameof(entity)} cannot be null");
                else
                    return;
            }

            var gen = this.Generator;
            List<string> names = typeof(TEntity).GetProperties().Select(x => x.Name).ToList();

            if (entity is IDictionary<string, object> dict)
            {
                foreach (var kvp in dict)
                {
                    if (names.IndexOf(kvp.Key) == -1)
                    {
                        if (throwException)
                            throw new ArgumentException($"invalid column \"{kvp.Key}\" in Table {schema.TableName}");
                        else
                            continue;
                    }

                    gen.Add(kvp.Key, kvp.Value);
                }
            }
            else
            {
                foreach (var propertyInfo in entity.GetType().GetProperties())
                {
                    if (names.IndexOf(propertyInfo.Name) == -1)
                    {
                        if (throwException)
                            throw new ArgumentException($"invalid column \"{propertyInfo.Name}\" in Table {schema.TableName}");
                        else
                            continue;
                    }

                    object value = propertyInfo.GetValue(entity);
                    gen.Add(propertyInfo.Name, value);
                }
            }

            Append(gen.Update(), RowOperation.PartialUpdate, gen.ToDictionary());
            gen.Clear();
        }

        /// <summary>
        /// Update rows 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="modifiedProperties">The properties are modified</param>
        /// <param name="where"></param>
        public void PartialUpdateOnSubmit(TEntity entity, Expression<Func<TEntity, object>> modifiedProperties, Expression<Func<TEntity, bool>> where)
        {
            if (entity == null)
                throw new ArgumentNullException($"argument {nameof(entity)} cannot be null");

            var style = Context.Style;
            List<string> names = new PropertyTranslator().Translate(modifiedProperties);
            string _where = new QueryTranslator(style).Translate(where);

            Dictionary<string, object> dict = new Dictionary<string, object>();
            List<Text.Expression> list = new List<Text.Expression>();
            foreach (var propertyInfo in entity.GetType().GetProperties())
            {
                if (names.IndexOf(propertyInfo.Name) == -1)
                    continue;

                object value = propertyInfo.GetValue(entity);
                string key = propertyInfo.Name;

                dict.Add(key, value);
                list.Add(key.AssignColumn(value));
            }

            SqlTemplate template = new SqlTemplate(formalName, SqlTemplateFormat.SingleLine);
            string update = template.Update(new Text.Expression(list).ToScript(style), _where);

            Append(update, RowOperation.PartialUpdate, dict);
        }

        private void Append(string sql, RowOperation operation, IDictionary<string, object> row)
        {
            Context.CodeBlock.AppendLine<TEntity>(operation, sql);

            var evt = new RowEvent
            {
                TypeName = typeof(TEntity).Name,
                Operation = operation,
                Row = row,
            };

            Context.RowEvents.Add(evt);
        }

        public override string ToString()
        {
            return formalName;
        }
    }
}
