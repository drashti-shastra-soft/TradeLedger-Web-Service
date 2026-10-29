namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IAddressService
    {
        Domain.AddressLister GetAll(Domain.AddressLister mLister);
         Domain.Address Upsert(Domain.Address mAddress);
        Domain.Address Get(int id);
        void Delete(int id, int loginUserId);
    }
}
