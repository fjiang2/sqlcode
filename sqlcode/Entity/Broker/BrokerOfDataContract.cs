﻿namespace Sys.Data.Entity
{
	class BrokerOfDataContract<TEntity> 
    {
        public static IDataContractBroker<TEntity> CreateBroker(EntityClassType clss)
        {
            if (clss == EntityClassType.ExtensionClass)
                return new BrokerOfDataContract1<TEntity>();
            else
                return new BrokerOfDataContract2<TEntity>();
        }
    }
}
