using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;

namespace TradeLedgerWebService.Service
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _repo;
        private readonly IContractItemRepository _itemRepository;
        private readonly IBrokerRepository _brokerepo;

        public ContractService(IContractRepository repo, IBrokerRepository brokerRepo, IContractItemRepository itemRepository)
        {
            _repo = repo;
            _brokerepo = brokerRepo;
            _itemRepository = itemRepository;
        }

        public Domain.ContractLister GetAll(Domain.ContractLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Contract Upsert(Domain.Contract mContract)
        {
            mContract = _repo.Upsert(mContract);
            if (mContract != null && mContract.Id > default(int))
            {
                var mSellerBroker = _brokerepo.Get(mContract.SellerBrokerId ?? 0);
                var mBuyerBroker = _brokerepo.Get(mContract.BuyerBrokerId ?? 0);

                foreach (var item in mContract.ContractItems)
                {
                    item.ContractId = mContract.Id;

                    if (mSellerBroker != null && mSellerBroker.Id > default(int))
                    {
                        item.SellerBrokerage = item.Qty * mSellerBroker.Brokerage;
                        mContract.SellerBrokerage += item.SellerBrokerage;
                    }

                    if (mBuyerBroker != null && mBuyerBroker.Id > default(int))
                    {
                        item.BuyerBrokerage = item.Qty * mBuyerBroker.Brokerage;
                        mContract.BuyerBrokerage += item.BuyerBrokerage;
                    }
                    _itemRepository.Upsert(item);
                }
            }
            return mContract;
        }


        public Domain.Contract Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }
    }
}
