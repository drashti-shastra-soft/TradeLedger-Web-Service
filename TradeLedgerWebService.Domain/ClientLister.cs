using System.Collections.Generic;

namespace TradeLedgerWebService.Domain
{
    public class ClientLister
    {
        public List<Client> List { get; set; } = new List<Client>();

        public Client SearchCriteria { get; set; } = new Client();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
