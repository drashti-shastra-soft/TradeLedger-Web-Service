using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLedgerWebService.Domain
{
    public class ContractItem : AuditTrail
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal? SellerBrokerage { get; set; }
        public decimal? BuyerBrokerage { get; set; }
        public int AuditId { get; set; }
    }
}
