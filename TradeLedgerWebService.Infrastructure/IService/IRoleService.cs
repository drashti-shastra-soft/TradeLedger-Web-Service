using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IService
{
    public interface IRoleService
    {
        List<Domain.Role> GetAll();
    }
}
