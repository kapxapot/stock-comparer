using System.Collections.Generic;
using System.Threading.Tasks;
using StockComparer.Models;

namespace StockComparer.Services.Interfaces
{
    public interface IExternalStockService
    {
        Task<IReadOnlyCollection<DailyStockData>> GetDailyStockData(string symbol);
    }
}
