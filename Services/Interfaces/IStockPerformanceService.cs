using System.Threading.Tasks;
using StockComparer.Models;

namespace StockComparer.Services.Interfaces
{
    public interface IStockPerformanceService
    {
        Task<StockPerformanceData> GetData(string symbol);
    }
}
