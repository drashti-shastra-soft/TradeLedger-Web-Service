using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IRepository
{
    public interface IBrokerRepository
    {
        Domain.BrokerLister GetAll(Domain.BrokerLister mLister);

        Domain.Broker Upsert(Domain.Broker mBroker);

        Domain.Broker Get(int id);

        void Delete(int id, int loginUserId);
        void UpdateAddress(int id, int addressId, int loginUserId);

        List<Domain.Broker> GetActiveBroker();
    }
}
