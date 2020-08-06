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
    public class PerformanceController : ControllerBase
    {
        private readonly IStockPerformanceService _stockPerformanceService;

        public PerformanceController(IStockPerformanceService stockPerformanceService)
        {
            _stockPerformanceService = stockPerformanceService;
        }

        // GET: api/Performance/IBM
        [HttpGet("{symbol}")]
        public async Task<ActionResult<IEnumerable<DayPerformanceData>>> Get(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return BadRequest();
            }

            var data = await _stockPerformanceService.GetData(symbol);

            if (!data.IsEmpty)
            {
                return data.Data.ToList();
            }

            return Problem(
                data.IsInvalid
                    ? data.Message
                    : $"Failed to calculate performance data for {symbol}"
            );
        }
    }
}
