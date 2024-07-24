using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenAI_API;
using OpenAI_API.Completions;

namespace portfolio.Services
{
    public interface IAnalyzeService {

    }

    public class AnalyzeService
    {
        private readonly OpenAIAPI _openAiApi;
        private readonly IConfiguration _config;
        private readonly IMarketDataService _marketDataService; 
        public AnalyzeService(IMarketDataService marketDataService, IConfiguration configuration)
        {
            _config = configuration;
            _openAiApi = new OpenAIAPI(_config["OpenAIKey"]);
            _marketDataService = marketDataService;
        }  

        public async Task<string> AnalyzeStockDataAsync(string symbol){
            var prompt = "You are a stock market expert with 30 years of experience, create a detail analysis of the following stock data\n";
            var stockData = "Realtime Stock Price\n" + await _marketDataService.GetRealTimeStockPriceAsync(symbol) + "Historical Stock Price\n" + await _marketDataService.GetHistoricalStockPriceAsync(symbol);
            CompletionRequest completionRequest  = new CompletionRequest();
            completionRequest .Model = OpenAI_API.Models.Model.GPT4;
            completionRequest .Prompt = prompt + stockData;
            completionRequest .MaxTokens = 256;
            var completions = await _openAiApi.Completions.CreateCompletionAsync(completionRequest);

            string response = "";

            foreach (var completion in completions.Completions)
            {
                response += completion.Text;
            }
            return response;
        }
    }
}