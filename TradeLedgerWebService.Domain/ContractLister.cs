using System.Collections.Generic;

namespace TradeLedgerWebService.Domain
{
    public class ContractLister
    {
        public List<Contract> List { get; set; } = new List<Contract>();

        public Contract SearchCriteria { get; set; } = new Contract();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
