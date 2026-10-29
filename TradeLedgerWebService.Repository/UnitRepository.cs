using TradeLedgerWebService.Infrastructure.IRepository;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace TradeLedgerWebService.Repository
{
    public class UnitRepository : IUnitRepository
    {
        public List<Domain.Unit> GetActiveUnit()
        {
            var mUnit = new List<Domain.Unit>();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efUnit = (from u in context.Units
                                  select new
                                  {
                                      Id = u.Id,
                                      Name = u.Name,
                                  }).AsEnumerable();

                    mUnit = JsonSerializer.Deserialize<List<Domain.Unit>>(JsonSerializer.Serialize(efUnit.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mUnit;
        }
    }
}
