using System.Security.Claims;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using portfolio.Services;

namespace portfolio.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/email")]
    public class EmailSNSController : ControllerBase
    {
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly IAnalysisService _analysisService;
        private string _topic = "SmartInvest-";
    
        public EmailSNSController(IAmazonSimpleNotificationService snsClient, IAnalysisService analysisService)
        {
            _snsClient = snsClient;        
            _analysisService = analysisService;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(int analysisId){
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var analysis = await _analysisService.GetAnalysisByIdAsync(analysisId);
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
    }
}