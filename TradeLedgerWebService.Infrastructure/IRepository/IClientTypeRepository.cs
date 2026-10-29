using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IRepository
{
    public interface IClientTypeRepository
    {
        List<Domain.ClientType> GetActiveClientTypes();
    }
}
