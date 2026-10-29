namespace TradeLedgerWebService.Domain
{
    public abstract class AuditTrail
    {
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
