using System;
using System.Collections.Generic;

namespace TradeLedgerWebService.Infrastructure.CustomException
{
    [Serializable]
    public class APIRequestFailedException : Exception
    {
        public APIRequestFailedException(string message)
            : base(message) { }
    }
}
