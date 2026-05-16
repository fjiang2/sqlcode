using System;
using System.Collections.Generic;
using System.Text;
using Sys.Data;
using Sys.Data.Entity;

namespace Sys.Data.EfCore
{
    public class EfContext : DbContext
    {
        public EfContext(IDbAgent agent)
            : base(agent)
        {
            EntityClassType = EntityClassType.EntityClass;
        }

        public override Table<TEntity> GetTable<TEntity>()
          where TEntity : class
        {
            return GetTable(t => new EfDataContractBroker<TEntity>());
        }
    }
}
