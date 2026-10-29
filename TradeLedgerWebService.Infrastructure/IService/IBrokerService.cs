using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IBrokerService
    {
        Domain.BrokerLister GetAll(Domain.BrokerLister mLister);

        Domain.Broker Upsert(Domain.Broker mBroker);

        Domain.Broker Get(int id);

        void Delete(int id, int loginUserId);
        void UpdateAddress(int id, int loginUserId, int addressId);
        List<Domain.Broker> GetActiveBroker();
    }
}
