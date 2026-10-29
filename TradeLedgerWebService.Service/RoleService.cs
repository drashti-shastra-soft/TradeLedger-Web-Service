using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;
using System.Collections.Generic;

namespace TradeLedgerWebService.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;
        public RoleService(IRoleRepository repo)
        {
            _repo = repo;
        }

        public List<Domain.Role> GetAll()
        {
            return _repo.GetAll();
        }
    }
}
