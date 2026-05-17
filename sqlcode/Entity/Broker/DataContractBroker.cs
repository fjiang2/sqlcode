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
    public class DataContractBroker<TEntity> : IDataContractBroker<TEntity>
    {
        const string dbo = "dbo";
        private readonly Type type;

        public List<string> NotMappedColumns { get; set; } = new List<string>();

        public DataContractBroker()
        {
            this.type = typeof(TEntity);
        }

        public virtual ITableSchema GetSchema(Type type)
        {
            string tableName = type.Name;
            string schemaName = dbo;
            var attrTable = Attribute.GetCustomAttribute(type, typeof(TableAttribute)) as TableAttribute;
            if (attrTable != null)
            {
                tableName = attrTable.Name;
                schemaName = attrTable.SchemaName ?? dbo;
            }

            string[] primaryKeys;
            var attrPrimaryKeys = Attribute.GetCustomAttribute(type, typeof(PrimaryKeysAttribute)) as PrimaryKeysAttribute;
            if (attrPrimaryKeys != null)
            {
                primaryKeys = attrPrimaryKeys.Columns;
            }
            else
            {
                List<string> keys = new List<string>();

                string column = type.GetProperties().FirstOrDefault()?.Name;
                if (column != null)
                    keys.Add(column);

                primaryKeys = keys.ToArray();
            }

            string[] identityKeys;
            var attrIdentityKeys = Attribute.GetCustomAttribute(type, typeof(IdentityKeysAttribute)) as IdentityKeysAttribute;
            if (attrIdentityKeys != null)
            {
                identityKeys = attrIdentityKeys.Columns;
            }
            else
            {
                identityKeys = new string[] { };
            }


            var attrNotMappedColumns = Attribute.GetCustomAttribute(type, typeof(NotMappedColumnsAttribute)) as NotMappedColumnsAttribute;
            if (attrNotMappedColumns != null)
            {
                NotMappedColumns.AddRange(attrNotMappedColumns.Columns);
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
            string[] keys;
            var attr = Attribute.GetCustomAttribute(type, typeof(T)) as T;
            if (attr != null)
            {
                keys = attr.Columns;
            }
            else
            {
                keys = new string[] { };
            }

            return keys;
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
