using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IItemService
    {
        Domain.ItemLister GetAll(Domain.ItemLister mLister);
        Domain.Item Upsert(Domain.Item mItem);
        Domain.Item Get(int id);
        void Delete(int id, int loginUserId);

        List<Domain.Item> GetActiveItems();
    }
}
