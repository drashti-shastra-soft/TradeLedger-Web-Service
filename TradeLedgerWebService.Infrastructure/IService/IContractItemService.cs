using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IContractItemService
    {
        Domain.ContractItemLister GetAll(Domain.ContractItemLister mLister);

        Domain.ContractItem Upsert(Domain.ContractItem mContractItem);

        List<Domain.ContractItem> Get(int id);

        void Delete(int id, int loginUserId);
    }
}
