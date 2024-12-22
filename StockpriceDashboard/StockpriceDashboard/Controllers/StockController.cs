using Microsoft.AspNetCore.Mvc;
using StockpriceDashboard.Services;

namespace StockpriceDashboard.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly StockService _stockService;

        public StockController(StockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetStockData(string symbol)
        {
            try
            {
                var data = await _stockService.GetStockDataAsync(symbol);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }


}
