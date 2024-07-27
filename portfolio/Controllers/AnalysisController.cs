using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using HtmlAgilityPack;
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
        private readonly IAmazonSimpleNotificationService _snsClient;
        private string _topic = "SmartInvest-";


        public AnalysisController(IAnalysisService analyzeService, IAmazonSimpleNotificationService snsClient)
        {
            _analyzeService = analyzeService;
            _snsClient = snsClient;
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

        [HttpPost("{id:int}/email")]
        public async Task<IActionResult> SendEmail(int id){
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            _topic += username;
            var analysis = await _analyzeService.GetAnalysisByIdAsync(id);
            if(analysis == null){
                return NotFound("Analysis not found");
            }
            var topicArnExists = await _snsClient.FindTopicAsync(_topic);
            string topicArn = "";
            if (topicArnExists == null)
            {
                var createTopicResponse = await _snsClient.CreateTopicAsync(_topic);
                topicArn = createTopicResponse.TopicArn;
                SubscribeRequest subscribeRequest = new SubscribeRequest()
                {
                    TopicArn = topicArn,
                    ReturnSubscriptionArn = true,
                    Protocol = "email",
                    Endpoint = userEmail,
                };

                var response = await _snsClient.SubscribeAsync(subscribeRequest);
            }
            else
            {
                topicArn = topicArnExists.TopicArn;
            }

            var message = analysis.Content;
            var doc = new HtmlDocument();
            doc.LoadHtml(message);
            message = doc.DocumentNode.InnerText;

            var subject = $"{analysis.StockSymbol}'s Financial Analysis";
            //create and publish a new message to the sns topic arn
            var publishRequest = new PublishRequest()
            {
                TopicArn = topicArn,
                Message = message,
                Subject = subject
            };

            await _snsClient.PublishAsync(publishRequest);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAnalysisAsync(int id){
            await _analyzeService.DeleteAnalysisAsync(id);
            return NoContent();
        }
    }
}