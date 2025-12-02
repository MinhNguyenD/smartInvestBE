using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using portfolio.Models;
using portfolio.Services;

namespace portfolio.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/portfolio")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPortfolio(){
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null){
                return Unauthorized();
            }
            var stockHoldings = new List<Stock>();
            try {
                stockHoldings = await _portfolioService.GetHoldings(userId!);
            } 
            catch(Exception e){
                return StatusCode(500, e.Message); 
            }
            return Ok(stockHoldings);
        }

        [HttpPost]
        public async Task<IActionResult> AddStockPortfolio(string symbol){
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null){
                return Unauthorized();
            }
            try {
                await _portfolioService.AddStockAsync(userId!, symbol);
            } 
            catch(Exception e){
                return StatusCode(500, e.Message); 
            }
            return Created();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveStockPortfolio(string symbol){
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null){
                return Unauthorized();
            }
            try {
                await _portfolioService.RemoveStockAsync(userId!, symbol);
            } 
            catch(Exception e){
                return StatusCode(500, e.Message); 
            }
            return NoContent();
        }
    }
}