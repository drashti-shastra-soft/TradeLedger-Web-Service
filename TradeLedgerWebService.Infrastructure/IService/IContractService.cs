using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IContractService
    {
        Domain.ContractLister GetAll(Domain.ContractLister mLister);
        Domain.Contract Upsert(Domain.Contract mContract);
        Domain.Contract Get(int id);
        void Delete(int id, int loginUserId);
    }
}
