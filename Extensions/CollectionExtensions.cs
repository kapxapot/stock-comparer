using System.Collections.Generic;
using System.Linq;
using StockComparer.Models;
using StockComparer.Models.Interfaces;

namespace StockComparer.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> CutPeriodChunk<T>(
            this IEnumerable<T> data,
            Period period
        ) where T : IDated =>
            data.Where(d => d.In(period));
    }
}
