using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;
using System.Collections.Generic;

namespace TradeLedgerWebService.Service
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _repo;

        public ClientService(IClientRepository repo)
        {
            _repo = repo;
        }

        public Domain.ClientLister GetAll(Domain.ClientLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Client Upsert(Domain.Client mClient)
        {
            return _repo.Upsert(mClient);
        }

        public Domain.Client Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.Client> GetActiveClient()
        {
            return _repo.GetActiveClient();
        }

        public void UpdateAddress(int id, int loginUserId, int addressId)
        {
            _repo.UpdateAddress(id, loginUserId,addressId);
        }

        public List<Domain.Client> GetActiveSellerClient()
        {
            return _repo.GetActiveSellerClient();
        }

        public List<Domain.Client> GetActiveBuyerClient()
        {
            return _repo.GetActiveBuyerClient();
        }
    }
}
