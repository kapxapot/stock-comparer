using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockComparer.Data;
using StockComparer.Models;
using StockComparer.Services.Interfaces;

namespace StockComparer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyController : ControllerBase
    {
        private readonly StockContext _context;
        private readonly IExternalStockService _externalStockService;

        public DailyController(StockContext context, IExternalStockService externalStockService)
        {
            _context = context;
            _externalStockService = externalStockService;
        }

        // GET: api/Daily/IBM
        [HttpGet("{symbol}")]
        public async Task<ActionResult<IEnumerable<DailyStockData>>> GetDailyStockData(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest();
            }

            var data = await _externalStockService.GetDailyStockData(symbol);

            return data.ToList();
        }
    }
}
