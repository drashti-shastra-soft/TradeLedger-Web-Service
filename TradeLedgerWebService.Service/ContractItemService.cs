using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;

namespace TradeLedgerWebService.Service
{
    public class ContractItemService : IContractItemService
    {
        private readonly IContractItemRepository _repo;

        public ContractItemService(IContractItemRepository repo)
        {
            _repo = repo;
        }

        public Domain.ContractItemLister GetAll(Domain.ContractItemLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.ContractItem Upsert(Domain.ContractItem mContractItem)
        {
            return _repo.Upsert(mContractItem);
        }

        public List<Domain.ContractItem> Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

    }
}
