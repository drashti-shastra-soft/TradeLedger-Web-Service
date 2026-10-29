namespace TradeLedgerWebService.Domain
{
    public class Client : AuditTrail
    {
        public int Id { get; set; }
        public int ClientTypeId { get; set; }
        public int CompanyId { get; set; }
        public string ClientType { get; set; }
        public string Company { get; set; }
        public int? AddressId { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Brand { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string PanNumber { get; set; }
        public string GSTNumber { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public int AuditId { get; set; }
    }
}
