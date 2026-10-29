using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IRepository
{
    public interface IClientRepository
    {
        Domain.ClientLister GetAll(Domain.ClientLister mLister);
        Domain.Client Upsert(Domain.Client mClient);
        Domain.Client Get(int id);
        void Delete(int id, int loginUserId);
        List<Domain.Client> GetActiveClient();
        void UpdateAddress(int id, int addressId, int loginUserId);
        List<Domain.Client> GetActiveSellerClient();
        List<Domain.Client> GetActiveBuyerClient();
    }   
}
