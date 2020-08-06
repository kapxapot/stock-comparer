using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StockComparer.Data;
using StockComparer.Extensions;
using StockComparer.Models;
using StockComparer.Services.Interfaces;

namespace StockComparer.Services
{
    public class StockDataService : IStockDataService
    {
        private readonly StockContext _context;
        private readonly IExternalStockService _externalStockService;
        private readonly ILogger<StockDataService> _logger;

        public StockDataService(StockContext context, IExternalStockService externalStockService, ILogger<StockDataService> logger)
        {
            _context = context;
            _externalStockService = externalStockService;
            _logger = logger;
        }

        public async Task<IEnumerable<DailyStockData>> GetLastWeekDailyStockData(string symbol)
        {
            // plan:
            // 1. try get from db
            // 2. if today data is not available, load from external service and save to db
            // 3. return last week data

            try
            {
                var data = await _externalStockService.GetDailyStockData(symbol);
                return data.CutLastWeekChunk();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve external stock data for {symbol}");
            }

            return Enumerable.Empty<DailyStockData>();
        }
    }
}
