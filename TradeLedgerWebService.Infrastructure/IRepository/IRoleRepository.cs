using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.IRepository
{
    public interface IRoleRepository
    {
        List<Domain.Role> GetAll();
    }
}
