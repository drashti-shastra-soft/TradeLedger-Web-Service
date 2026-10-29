using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;
using System.Collections.Generic;

namespace TradeLedgerWebService.Service
{
    public class BrokerService : IBrokerService
    {
        private readonly IBrokerRepository _repo;

        public BrokerService(IBrokerRepository repo)
        {
            _repo = repo;
        }

        public Domain.BrokerLister GetAll(Domain.BrokerLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Broker Upsert(Domain.Broker mBroker)
        {
            return _repo.Upsert(mBroker);
        }

        public Domain.Broker Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public void UpdateAddress(int id, int loginUserId, int addressId)
        {
            _repo.UpdateAddress(id, loginUserId, addressId);
        }

        public List<Domain.Broker> GetActiveBroker()
        {
            return _repo.GetActiveBroker();
        }


    }

}
