using System;

namespace StockComparer.Exceptions
{
    public class CriticalException : Exception
    {
        public CriticalException(string message) : base(message)
        {
        }
    }
}
