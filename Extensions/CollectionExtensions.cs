using System;
using System.Collections.Generic;
using System.Linq;
using StockComparer.Models.Interfaces;

namespace StockComparer.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> CutLastWeekChunk<T>(
            this IEnumerable<T> data
        ) where T : IDated
        {
            var end = DateTime.UtcNow.Date;
            var start = end.AddDays(-6);

            return data.CutPeriodChunk(start, end);
        }

        public static IEnumerable<T> CutPeriodChunk<T>(
            this IEnumerable<T> data,
            DateTime start,
            DateTime end
        ) where T : IDated =>
            data.Where(d => d.Date >= start && d.Date <= end);
    }
}
