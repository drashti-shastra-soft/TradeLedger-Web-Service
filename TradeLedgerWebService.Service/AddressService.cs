using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;

namespace TradeLedgerWebService.Service
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repo;
        private readonly IClientRepository _clientRepo;
        private readonly IBrokerRepository _brokerRepo;

        public AddressService(IAddressRepository repo, IClientRepository clientRepo, IBrokerRepository brokerRepo)
        {
            _repo = repo;
            _clientRepo = clientRepo;
            _brokerRepo = brokerRepo;
        }

        public Domain.AddressLister GetAll(Domain.AddressLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Address Upsert(Domain.Address mAddress)
        {
            var mCreatedAddress = _repo.Upsert(mAddress);
            if (mCreatedAddress != null && mCreatedAddress.Id > default(int))
            {
                if (mCreatedAddress.ClientId > default(int))
                {
                    _clientRepo.UpdateAddress(mCreatedAddress.ClientId, mCreatedAddress.Id, mCreatedAddress.CreatedBy);
                }

                else if (mCreatedAddress.BrokerId > default(int))
                {
                    _brokerRepo.UpdateAddress(mCreatedAddress.BrokerId, mCreatedAddress.Id, mCreatedAddress.CreatedBy);
                }
            }
            return mCreatedAddress;
        }

        public Domain.Address Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

    }
}
