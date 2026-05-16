using System;

namespace Sys.Data.Entity
{
    class BrokerOfDataContract
    {

        public static IDataContractBroker<TEntity> CreateBroker<TEntity>(EntityClassType classType)
        {
            switch (classType)
            {
                case EntityClassType.ExtensionClass:
                    return new BrokerOfDataContract1<TEntity>();

                case EntityClassType.SingleClass:
                    return new BrokerOfDataContract2<TEntity>();
            }

            return new DataContractBroker<TEntity>();
        }
    }
}
