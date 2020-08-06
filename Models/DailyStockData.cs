using System;
using StockComparer.Models.Interfaces;

namespace StockComparer.Models
{
    public class DailyStockData : IDated
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
    }
}
