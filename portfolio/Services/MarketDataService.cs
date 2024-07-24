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
        Task<string?> GetHistoricalStockPriceAsync(string symbol)
    }

    public class MarketDataService : IMarketDataService
    {
        private HttpClient _httpClient;
        private IConfiguration _config;
        public MarketDataService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        
        public async Task<Stock> FindStockBySymbolAsync(string symbol)
        {
            try
            {
                var result = await _httpClient.GetAsync($"https://financialmodelingprep.com/api/v3/profile/{symbol}?apikey={_config["MarketDataApiKey"]}");
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var tasks = JsonConvert.DeserializeObject<MarketDataStock[]>(content);
                    var stock = tasks[0];
                    if (stock != null)
                    {
                        return stock.ToStockFromMarketData();;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<string?> GetRealTimeStockPriceAsync(string symbol)
        {
            try
            {
                var result = await _httpClient.GetAsync($"https://financialmodelingprep.com/api/v3/quote/{symbol}?apikey={_config["MarketDataApiKey"]}");
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
                return null;
            }
        }

        public async Task<string?> GetHistoricalStockPriceAsync(string symbol)
        {
            try
            {
                var result = await _httpClient.GetAsync($"https://financialmodelingprep.com/api/v3/historical-price-full/{symbol}?apikey={_config["MarketDataApiKey"]}");
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
                return null;
            }
        }
    }
}