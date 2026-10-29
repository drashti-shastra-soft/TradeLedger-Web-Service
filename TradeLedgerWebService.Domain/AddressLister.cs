using System.Collections.Generic;

namespace TradeLedgerWebService.Domain
{
    public class AddressLister
    {
        public List<Address> List { get; set; } = new List<Address>();

        public Address SearchCriteria { get; set; } = new Address();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
