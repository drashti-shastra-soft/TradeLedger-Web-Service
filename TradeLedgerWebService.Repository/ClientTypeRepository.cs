using TradeLedgerWebService.Infrastructure.IRepository;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace TradeLedgerWebService.Repository
{
    public class ClientTypeRepository : IClientTypeRepository
    {
        public List<Domain.ClientType> GetActiveClientTypes()
        {
            var mClientTypes = new List<Domain.ClientType>();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efClientTypes = (from ct in context.ClientTypes
                                         select new
                                         {
                                             Id = ct.Id,
                                             Name = ct.Name,
                                         }).AsEnumerable();

                    mClientTypes = JsonSerializer.Deserialize<List<Domain.ClientType>>(JsonSerializer.Serialize(efClientTypes.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mClientTypes;
        }
    }
}
