using StockComparer.Models;
using StockComparer.Models.Interfaces;

namespace StockComparer.Extensions
{
    public static class DatedExtensions
    {
        public static bool In(this IDated dated, Period period) =>
            dated.Date >= period.Start && dated.Date <= period.End;
    }
}
