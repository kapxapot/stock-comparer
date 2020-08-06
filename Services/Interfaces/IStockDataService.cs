using System.Collections.Generic;
using System.Threading.Tasks;
using StockComparer.Models;

namespace StockComparer.Services.Interfaces
{
    public interface IStockDataService
    {
        Task<IEnumerable<DailyStockData>> GetLastWeekDailyStockData(string symbol);
    }
}
