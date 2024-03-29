﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;

namespace Sys.Data.Entity
{

    class BrokerOfDataContract2<TEntity> : IDataContractBroker<TEntity>
    {
        private readonly Type type;
        public ITableSchema Schema { get; }

        public BrokerOfDataContract2()
        {
            this.type = typeof(TEntity);
            this.Schema = type.GetTableSchemaFromType();
        }

        public ITableSchema GetSchmea(Type type)
        {
            return type.GetTableSchemaFromType();
        }

        public IDictionary<string, object> ToDictionary(TEntity entity)
        {
            if (entity is IEntityRow entityRow)
                return entityRow.ToDictionary();
            else
                throw new Exception($"Invalid IEntityRow: {entity}");
        }


        public List<TEntity> ToList(DataTable dt)
        {
            //typeof(IEntityRow).IsAssignableFrom(typeof(TEntity))

            List<TEntity> list = new List<TEntity>();
            foreach (DataRow row in dt.Rows)
            {
                //dc2 requires constructor public EntityClass(DataRow row) {...}
                TEntity obj = (TEntity)Activator.CreateInstance(type, new object[] { row });
                list.Add(obj);
            }
            return list;
        }

    }
}
