using Microsoft.EntityFrameworkCore;
using StockComparer.Models;

namespace StockComparer.Data
{
    public class StockContext : DbContext
    {
        public StockContext(DbContextOptions<StockContext> options) : base(options)
        {
        }

        public DbSet<DailyStockData> DailyStockData { get; set; }
    }
}
