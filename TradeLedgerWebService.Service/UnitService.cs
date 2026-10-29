using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;
using System.Collections.Generic;

namespace TradeLedgerWebService.Service
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _repo;
        public UnitService(IUnitRepository repo)
        {
            _repo = repo;
        }

        public List<Domain.Unit> GetActiveUnit()
        {
            return _repo.GetActiveUnit();
        }
    }
}
