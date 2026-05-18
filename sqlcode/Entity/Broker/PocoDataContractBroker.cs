using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using Sys.Data.Attributes;

namespace Sys.Data.Entity
{
    /// <summary>
    /// Default broker of data contract
    /// Table name is the same as class name, and primary key is the first property of the class.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PocoDataContractBroker<TEntity> : IDataContractBroker<TEntity>
    {
        private readonly Type type;

        public List<string> NotMappedColumns { get; set; } = new List<string>();

        public PocoDataContractBroker()
        {
            this.type = typeof(TEntity);
        }

        public virtual ITableSchema GetSchema(Type type)
        {
            // Check if the schema is already registered in EntitySchema
            var schema = PocoSchema.GetSchema(type);
            if (schema != null)
            {
                return schema;
            }

            string tableName = type.Name;
            string schemaName = null;
            if (Attribute.GetCustomAttribute(type, typeof(TableAttribute)) is TableAttribute attrTable)
            {
                tableName = attrTable.Name;
                schemaName = attrTable.SchemaName;
            }

            string[] primaryKeys = GetColumns<PrimaryKeyAttribute>();

            // If primary keys are not defined, use the first property as the primary key by default
            if (primaryKeys.Length == 0)
            {
                List<string> keys = new List<string>();

                string column = type.GetProperties().FirstOrDefault()?.Name;
                if (column != null)
                    keys.Add(column);

                primaryKeys = keys.ToArray();
            }

            string[] identityKeys = GetColumns<IdentityKeyAttribute>();

            var notMappedColumns = GetColumns<NotMappedColumnAttribute>();
            if (notMappedColumns.Length > 0)
            {
                NotMappedColumns.AddRange(notMappedColumns);
            }

            return new TableSchema
            {
                TableName = tableName,
                SchemaName = schemaName,
                PrimaryKeys = primaryKeys,
                IdentityKeys = identityKeys,
            };
        }


        private string[] GetColumns<T>() where T : Attribute, IColumnsAttribute
        {
            string[] columns;
            if (Attribute.GetCustomAttribute(type, typeof(T)) is T attr)
            {
                columns = attr.Columns;
            }
            else
            {
                columns = new string[] { };
            }

            return columns;
        }


        public virtual IDictionary<string, object> ToDictionary(TEntity entity)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                string columnName = propertyInfo.Name;
                if (NotMappedColumns.Contains(columnName))
                {
                    continue;
                }

                object value = propertyInfo.GetValue(entity) ?? DBNull.Value;
                dict.Add(columnName, value);
            }

            return dict;
        }


        public virtual List<TEntity> ToList(DataTable dt)
        {
            PropertyInfo[] properties = type.GetProperties();

            List<TEntity> list = new List<TEntity>();
            foreach (DataRow row in dt.Rows)
            {
                TEntity entity = (TEntity)Activator.CreateInstance(type);
                foreach (PropertyInfo propertyInfo in properties)
                {
                    string columnName = propertyInfo.Name;
                    if (NotMappedColumns.Contains(columnName))
                    {
                        continue;
                    }

                    if (dt.Columns.Contains(columnName))
                    {
                        object value = row[columnName];
                        if (value == DBNull.Value)
                        {
                            value = null;
                        }

                        propertyInfo.SetValue(entity, value);
                    }
                    else
                    {
                        throw new Exception($"Column={columnName} in entity={type.Name} not found in DataTable: {dt.TableName}.");
                    }
                }
            }

            return list;
        }

    }
}
