using System;
using StockComparer.Models.Interfaces;

namespace StockComparer.Models
{
    public class DayPerformanceData : IDated
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public decimal Delta { get; set; }
        public decimal SpyValue { get; set; }
        public decimal SpyDelta { get; set; }
    }
}
