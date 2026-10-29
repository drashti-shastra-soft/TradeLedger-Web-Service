using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;
using System.Collections.Generic;

namespace TradeLedgerWebService.Service
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repo;

        public ItemService(IItemRepository repo)
        {
            _repo = repo;
        }

        public Domain.ItemLister GetAll(Domain.ItemLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Item Upsert(Domain.Item mItem)
        {
            return _repo.Upsert(mItem);
        }

        public Domain.Item Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.Item> GetActiveItems()
        {
            return _repo.GetActiveItems();
        }
    }
}
