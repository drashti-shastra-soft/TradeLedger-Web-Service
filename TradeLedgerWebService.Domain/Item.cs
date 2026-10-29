namespace TradeLedgerWebService.Domain
{
    public class Item : AuditTrail
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string HSNCode { get; set; }
        public string Status { get; set; }
        public int UnitId { get; set; }
        public bool IsActive { get; set; }
        public int AuditId { get; set; }
    }
}
