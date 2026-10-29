using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeLedgerWebService.Infrastructure.Utils
{
    public static class Enums
    {
        public enum Role
        {
            System_Owner = 1,
            Super_Admin = 2,
            Admin = 3,
            Employee = 4,
        }
    }
}
