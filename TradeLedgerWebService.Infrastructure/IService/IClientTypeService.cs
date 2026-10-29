using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IClientTypeService
    {
        List<Domain.ClientType> GetActiveClientTypes();
    }
}
