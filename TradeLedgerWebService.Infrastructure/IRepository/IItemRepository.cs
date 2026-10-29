using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IRepository
{
    public interface IItemRepository
    {
        Domain.ItemLister GetAll(Domain.ItemLister mLister);
        Domain.Item Upsert(Domain.Item mItem);
        Domain.Item Get(int id);
        void Delete(int id, int loginUserId);

        List<Domain.Item> GetActiveItems();
    }
}
