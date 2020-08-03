using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockComparer.Data;
using StockComparer.Models;

namespace StockComparer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyController : ControllerBase
    {
        private readonly StockContext _context;

        public DailyController(StockContext context)
        {
            _context = context;
        }

        // GET: api/Daily/IBM
        [HttpGet("{symbol}")]
        public async Task<ActionResult<IEnumerable<DailyStockData>>> GetDailyStockData(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest();
            }

            return await _context.DailyStockData.ToListAsync();
        }
    }
}
