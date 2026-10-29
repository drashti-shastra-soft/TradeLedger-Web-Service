using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IUnitService
    {
        List<Domain.Unit> GetActiveUnit();
    }
}
