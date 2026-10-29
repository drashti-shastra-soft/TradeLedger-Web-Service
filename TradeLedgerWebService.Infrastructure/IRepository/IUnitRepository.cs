using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IRepository
{
    public interface IUnitRepository
    {
        List<Domain.Unit> GetActiveUnit();
    }
}
