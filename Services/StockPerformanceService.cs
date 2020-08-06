using System;
using System.Linq;
using System.Threading.Tasks;
using StockComparer.Models;
using StockComparer.Services.Interfaces;

namespace StockComparer.Services
{
    public class StockPerformanceService : IStockPerformanceService
    {
        private const string SpySymbol = "SPY";

        private readonly IStockDataService _stockDataService;

        public StockPerformanceService(IStockDataService stockDataService)
        {
            _stockDataService = stockDataService;
        }

        public async Task<StockPerformanceData> GetData(string symbol)
        {
            try
            {
                return await TryGetData(symbol);
            }
            catch (Exception ex)
            {
                return StockPerformanceData.Invalid(ex.Message);
            }
        }

        private async Task<StockPerformanceData> TryGetData(string symbol)
        {
            var stockData = await _stockDataService.GetLastWeekDailyStockData(symbol);
            var spyData = await _stockDataService.GetLastWeekDailyStockData(SpySymbol);

            if (!stockData.Any() || !spyData.Any())
            {
                return StockPerformanceData.Invalid("No data.");
            }

            var orderedStockData = stockData.OrderBy(d => d.Date);

            var data = new StockPerformanceData();

            foreach (var stockDay in orderedStockData)
            {
                if (!spyData.Any(d => d.Date == stockDay.Date))
                {
                    return StockPerformanceData.Invalid($"Inconsistent {symbol} / SPY data.");
                }

                var spyDay = spyData.First(d => d.Date == stockDay.Date);

                data.AddDay(stockDay, spyDay);
            }

            data.Calculate();

            return data;
        }
    }
}
