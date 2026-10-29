using TradeLedgerWebService.Infrastructure.IRepository;
using TradeLedgerWebService.Infrastructure.IService;
using System.Collections.Generic;

namespace TradeLedgerWebService.Service
{
    public class ClientTypeService : IClientTypeService
    {
        private readonly IClientTypeRepository _repository;
        public ClientTypeService(IClientTypeRepository repository)
        {
            _repository = repository;
        }

        public List<Domain.ClientType> GetActiveClientTypes()
        {
            return _repository.GetActiveClientTypes();
        }
    }
}
