using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using portfolio.Services;

namespace portfolio.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/analyses")]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisService _analyzeService;

        public AnalysisController(IAnalysisService analyzeService)
        {
            _analyzeService = analyzeService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAnalysesAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null){
                return Unauthorized();
            }
            var analyses = await _analyzeService.GetAnalysesAsync(userId);
            return Ok(analyses);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAnalysisByIdAsync(int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null){
                return Unauthorized();
            }
            try{
                var analysis = await _analyzeService.GetAnalysisByIdAsync(id);
                if(analysis == null){
                    return NotFound();
                }
                return Ok(analysis);
            }
            catch(Exception e){
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("/symbol/{symbol}")]
        public async Task<IActionResult> GetAnalysisByStockAsync(string symbol)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null){
                return Unauthorized();
            }
            try{
                var analysis = await _analyzeService.GetAnalysisByStockAsync(userId, symbol);
                if(analysis == null){
                    return NotFound();
                }
                return Ok(analysis);
            }
            catch(Exception e){
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnalysisAsync(string symbol){
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if(userId == null){
                return Unauthorized();
            }
            try {
                var analysis = await _analyzeService.CreateStockAnalysisAsync(userId, symbol);
                if(analysis == null){
                    return StatusCode(500, "Error creating analysis");
                }
                return Ok(analysis);    
            }
            catch(Exception e){
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAnalysisAsync(int id){
            await _analyzeService.DeleteAnalysisAsync(id);
            return NoContent();
        }
    }
}