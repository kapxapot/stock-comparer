using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockComparer.Models;
using StockComparer.Services.Interfaces;

namespace StockComparer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyController : ControllerBase
    {
        private readonly IStockDataService _stockDataService;

        public DailyController(IStockDataService stockDataService)
        {
            _stockDataService = stockDataService;
        }

        // GET: api/Daily/IBM
        [HttpGet("{symbol}")]
        public async Task<ActionResult<IEnumerable<DailyStockData>>> GetDailyStockData(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest();
            }

            var data = (await _stockDataService.GetLastWeekDailyStockData(symbol)).ToList();

            if (data.Any())
            {
                return data;
            }

            return NotFound();
        }
    }
}
