using System;
using System.Collections.Generic;
using System.Linq;
using StockComparer.Extensions;

namespace StockComparer.Models
{
    public class StockPerformanceData
    {
        private List<DayPerformanceData> _data;
        private Func<DailyStockData, decimal> _extractStockValue;
        private string _message;

        public StockPerformanceData()
        {
            _data = new List<DayPerformanceData>();
            _extractStockValue = ExtractStockValue;
            _message = string.Empty;
        }

        public IEnumerable<DayPerformanceData> Data => _data;

        public string Message => _message;

        public StockPerformanceData WithValueExtractor(Func<DailyStockData, decimal> func)
        {
            _extractStockValue = func;
            return this;
        }

        public StockPerformanceData WithMessage(string message)
        {
            _message = message;
            return this;
        }

        /// <summary>
        /// Todo: Extract this to builder class.
        /// </summary>
        public void AddDay(DailyStockData stockData, DailyStockData spyData)
        {
            if (stockData.Date != spyData.Date)
            {
                throw new ArgumentException(
                    "Stock data and SPY data must be of the same date."
                );
            }

            var dayData = new DayPerformanceData
            {
                Date = stockData.Date,
                Value = _extractStockValue(stockData),
                SpyValue = _extractStockValue(spyData)
            };

            _data.Add(dayData);
        }

        /// <summary>
        /// Todo: Extract this to builder class.
        /// </summary>
        public void Calculate()
        {
            if (!_data.Any())
            {
                throw new InvalidOperationException("No data. Add days first.");
            }

            var days = _data.OrderBy(d => d.Date);
            var firstDay = days.First();

            foreach (var day in days)
            {
                day.Delta = day.Value.Delta(firstDay.Value).ToPercent();
                day.SpyDelta = day.SpyValue.Delta(firstDay.SpyValue).ToPercent();
            }
        }

        public bool IsEmpty => !_data.Any();

        public bool IsInvalid => IsEmpty && !string.IsNullOrWhiteSpace(_message);

        public static StockPerformanceData Empty => new StockPerformanceData();

        public static StockPerformanceData Invalid(string message) =>
            Empty.WithMessage(message);

        private decimal ExtractStockValue(DailyStockData data)
        {
            return data.High;
        }
    }
}
