using System;
using System.Collections.Generic;
using System.Data;

namespace Sys.Data.Entity
{
    public interface IDataContractBroker<TEntity>
    {
        ITableSchema GetSchema(Type type);
        IDictionary<string, object> ToDictionary(TEntity entity);
        List<TEntity> ToList(DataTable dt);
    }
}