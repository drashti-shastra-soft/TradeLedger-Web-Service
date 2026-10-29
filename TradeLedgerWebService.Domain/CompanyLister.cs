using System.Collections.Generic;

namespace TradeLedgerWebService.Domain
{
    public class CompanyLister
    {
        public List<Company> List { get; set; } = new List<Company>();

        public Company SearchCriteria { get; set; } = new Company();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
