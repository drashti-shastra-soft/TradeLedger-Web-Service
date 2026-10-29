using System.Collections.Generic;

namespace TradeLedgerWebService.Domain
{
    public class ItemLister
    {
        public List<Item> List { get; set; } = new List<Item>();

        public Item SearchCriteria { get; set; } = new Item();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
