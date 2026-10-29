using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLedgerWebService.Domain
{
    public class ContractItemLister
    {
        public List<ContractItem> List { get; set; } = new List<ContractItem>();

        public ContractItem SearchCriteria { get; set; } = new ContractItem();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
