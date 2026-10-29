using TradeLedgerWebService.Infrastructure.IRepository;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace TradeLedgerWebService.Repository
{
    public class RoleRepository : IRoleRepository
    {
        public List<Domain.Role> GetAll()
        {
            var mRoles = new List<Domain.Role>();
            try
            {
                using (var context = new Persistence.TradeLedgerDB())
                {
                    var efRoles = (from r in context.Roles
                                   select new
                                   {
                                       Id = r.Id,
                                       Name = r.Name
                                   }).AsEnumerable();

                    mRoles = JsonSerializer.Deserialize<List<Domain.Role>>(JsonSerializer.Serialize(efRoles.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mRoles;
        }
    }
}
