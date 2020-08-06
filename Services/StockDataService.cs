using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            var period = Period.LastWeek;

            // 1. try get from db
            var data = await GetFromDb(symbol, period);

            // 2. if last stock day data is not available,
            // load from external service and save to db
            var stockDays = period.StockDays;

            if (!stockDays.Any())
            {
                throw new Exception("Invalid period: no stock days.");
            }

            var lastStockDay = stockDays.OrderBy(d => d).Last();

            if (data.Any(d => d.Date == lastStockDay))
            {
                _logger.LogInformation($"Fetched {symbol} data from the db.");

                return data;
            }

            try
            {
                data = await _externalStockService.GetDailyStockData(symbol);

                _logger.LogInformation($"Fetched {symbol} data from the external API.");
            }
            catch (Exception ex)
            {
                throw new AggregateException(
                    $"Failed to retrieve external stock data for {symbol}.",
                    ex
                );
            }

            await SaveToDb(data);

            // 3. return last week data
            return data.CutPeriodChunk(period);
        }

        private async Task<IEnumerable<DailyStockData>> GetFromDb(string symbol, Period period)
        {
            return await _context.DailyStockData
                .Where(d => d.Symbol == symbol && d.Date >= period.Start && d.Date <= period.End)
                .ToListAsync();
        }

        private async Task SaveToDb(IEnumerable<DailyStockData> data)
        {
            foreach (var datum in data)
            {
                if (_context.DailyStockData.Any(
                    d => d.Symbol == datum.Symbol && d.Date == datum.Date
                ))
                {
                    continue;
                }

                await _context.DailyStockData.AddAsync(datum);
            }

            await _context.SaveChangesAsync();
        }
    }
}
