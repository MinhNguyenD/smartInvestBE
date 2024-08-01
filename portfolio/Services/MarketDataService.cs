using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using portfolio.Dtos;
using portfolio.Mappers;
using portfolio.Models;

namespace portfolio.Services
{
    public interface IMarketDataService
    {
        Task<Stock> FindStockBySymbolAsync(string symbol);
        Task<string?> GetRealTimeStockPriceAsync(string symbol);
        Task<string?> GetFinancialStatement(string symbol);
        Task<string?> GetKeyMetrics(string symbol);
    }

    public class MarketDataService : IMarketDataService
    {
        private HttpClient _httpClient;
        private IConfiguration _config;
        private readonly string _marketDataKey;

        public MarketDataService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _marketDataKey = config["MarketData:Key"];
        }
        
        public async Task<Stock> FindStockBySymbolAsync(string symbol)
        {
            try
            {
                var result = await _httpClient.GetAsync($"https://financialmodelingprep.com/api/v3/profile/{symbol}?apikey={_marketDataKey}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var tasks = JsonConvert.DeserializeObject<MarketDataStock[]>(content);
                    var stock = tasks[0];
                    if (stock != null)
                    {
                        return stock.ToStockFromMarketData();
                    }
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception($"Market data api request failed");
            }
        }

        public async Task<string?> GetRealTimeStockPriceAsync(string symbol)
        {
            var apiUri = $"https://financialmodelingprep.com/api/v3/quote/{symbol}?apikey={_marketDataKey}";
            return await GetMarketData(apiUri);
        }

        public async Task<string?> GetFinancialStatement(string symbol)
        {
            var apiUri = $"https://financialmodelingprep.com/api/v3/financials/income-statement/{symbol}?apikey={_marketDataKey}";
            return await GetMarketData(apiUri);
        }

        public async Task<string?> GetKeyMetrics(string symbol)
        {
            var apiUri = $"https://financialmodelingprep.com/api/v3/key-metrics/{symbol}?apikey={_marketDataKey}";
            return await GetMarketData(apiUri);
        }

        private async Task<string?> GetMarketData(string apiUri){
            try
            {
                var result = await _httpClient.GetAsync(apiUri);
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    if(content.IsNullOrEmpty()){
                        return null;
                    }
                    return content;
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception($"Market data api request failed");
            }
        }
    }
}