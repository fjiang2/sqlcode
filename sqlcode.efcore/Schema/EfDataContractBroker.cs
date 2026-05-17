using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Sys.Data;
using Sys.Data.Entity;

namespace Sys.Data.EfCore.Schema
{
    public class EfDataContractBroker<TEntity> : DataContractBroker<TEntity>
    {
        public EfDataContractBroker()
        {
            base.NotMappedColumns = typeof(TEntity).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), true).Length > 0)
                .Select(p => p.Name)
                .ToList();
        }

        public override ITableSchema GetSchema(Type type)
        {
            return new TableSchema
            {
                TableName = GetTableName(type),
                PrimaryKeys = GetPrimaryKeys(type)
            };
        }

        private static T? GetAttribute<T>(Type type) where T : Attribute
        {
            var attr = Attribute.GetCustomAttribute(type, typeof(T));
            if (attr != null)
            {
                return (T)attr;
            }

            return default(T);
        }


        private static string GetTableName(Type type)
        {
            var tableAttr = GetAttribute<TableAttribute>(type);
            if (tableAttr != null)
            {
                return tableAttr.Name;
            }

            return type.Name;
        }

        private static string[] GetPrimaryKeys(Type type)
        {
            var keysAttr = GetAttribute<PrimaryKeyAttribute>(type);
            if (keysAttr != null)
            {
                return keysAttr.PropertyNames.ToArray();
            }
            else
            {
                return type.GetProperties()
                    .Where(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0)
                    .Select(p => p.Name)
                    .ToArray();
            }

        }
    }
}
