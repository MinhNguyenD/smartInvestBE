using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using portfolio.Data;
using portfolio.Models;

namespace portfolio.Services
{
    public interface IAnalysisService {
        Task<Analysis?> CreateStockAnalysisAsync(string userId, string symbol);
        Task<List<Analysis>> GetAnalysesAsync(string userId);
        Task<Analysis?> GetAnalysisByIdAsync(int id);
        Task<Analysis?> GetAnalysisByStockAsync(string userId, string symbol);
        Task<Analysis?> DeleteAnalysisAsync(int analysisId);
    }

    public class AnalysisService : IAnalysisService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IMarketDataService _marketDataService; 

        public AnalysisService(ApplicationDBContext dbContext, IConfiguration configuration, IMarketDataService marketDataService,HttpClient httpClient)
        {
            _dbContext = dbContext;
            _config = configuration;
            _httpClient = httpClient;
            _marketDataService = marketDataService;
            var authToken = "Bearer " + _config["OpenAI:Key"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", authToken);
        }  

        public async Task<Analysis?> CreateStockAnalysisAsync(string userId, string symbol){
            var stockData = "Realtime stock price:\n" + await _marketDataService.GetRealTimeStockPriceAsync(symbol) + "Financial statement:\n" + await _marketDataService.GetFinancialStatement(symbol) + "Key metrics:\n" + await _marketDataService.GetKeyMetrics(symbol);
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a stock market expert with 30 years of experience." },
                    new { role = "user", content = $@"Analyze the following stock data and provide a detailed report. 
Your report should include not only a summary of the data but also insights, trends, and actionable analysis that an investor might find useful. 
Consider factors like price movements, trading volume, financial health, valuation metrics, and market conditions. 
Highlight any potential risks or opportunities, and offer recommendations for investors. :\n{stockData}\nPlease format the response with HTML tags and put it in an empty <div> element (div has not style), ensuring the response is ready to be rendered directly on a React, Typescript, Tailwind css web page without any escape characters or unnecessary formatting symbols." }
                }
            };
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseBody);
            var analysisResponse = responseJson!.choices[0].message.content.ToString();
            var stock = await _dbContext.Stocks.FirstOrDefaultAsync(s => s.Symbol.Equals(symbol)); 
            if(stock == null){
                stock = await _marketDataService.FindStockBySymbolAsync(symbol);
                if(stock == null){
                    throw new Exception("Stock not found");
                }
                await _dbContext.Stocks.AddAsync(stock);
                await _dbContext.SaveChangesAsync();
            }

            Analysis analysis = new Analysis {
                UserId = userId,
                StockSymbol = symbol,
                DateCreated =  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Content = analysisResponse,
                StockId = stock.Id
            };

            await _dbContext.Analyses.AddAsync(analysis);
            await _dbContext.SaveChangesAsync();
            return analysis;
        }

        public async Task<List<Analysis>> GetAnalysesAsync(string userId){
            var analyses = await _dbContext.Analyses.Where(a => a.UserId.Equals(userId)).ToListAsync();
            return analyses;
        }

        public async Task<Analysis?> GetAnalysisByStockAsync(string userId, string symbol){
            var stock = await _dbContext.Stocks.FirstOrDefaultAsync(s => s.Symbol.Equals(symbol));  
            if(stock == null){
                stock = await _marketDataService.FindStockBySymbolAsync(symbol);
                if(stock == null){
                    throw new Exception("Stock not found");
                }
                await _dbContext.Stocks.AddAsync(stock);
                await _dbContext.SaveChangesAsync();
            }
            var analysis = _dbContext.Analyses.FirstOrDefault(a => a.UserId.Equals(userId) && a.StockId == stock!.Id);
            return analysis;
        }

        public async Task<Analysis?> GetAnalysisByIdAsync(int id){
            var analysis = await _dbContext.Analyses.FindAsync(id);
            return analysis;
        }

        public async Task<Analysis?> DeleteAnalysisAsync(int analysisId){
            var analysis = await _dbContext.Analyses.FindAsync(analysisId);
             if (analysis is null)
            {
                return null;
            }
            _dbContext.Analyses.Remove(analysis);
            await _dbContext.SaveChangesAsync();
            return analysis;
        }
    }
}