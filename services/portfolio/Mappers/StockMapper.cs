using portfolio.Dtos;
using portfolio.Models;

namespace portfolio.Mappers;

public static class StockMapper
{
    public static StockDto ToStockDto(this Stock stockModel)
    {
        return new StockDto
        {
            Id = stockModel.Id,
            Symbol = stockModel.Symbol,
            CompanyName = stockModel.CompanyName,
            Price = stockModel.Price,
            LastDiv = stockModel.LastDiv,
            Industry = stockModel.Industry,
            MarketCap = stockModel.MarketCap,
        };
    }

    public static Stock ToStockFromCreateDTO(this CreateStockDto stockDto)
    {
        return new Stock
        {
            Symbol = stockDto.Symbol,
            CompanyName = stockDto.CompanyName,
            Price = stockDto.Price,
            LastDiv = stockDto.LastDiv,
            Industry = stockDto.Industry,
            MarketCap = stockDto.MarketCap
        };
    }

    public static Stock ToStockFromMarketData(this MarketDataStock marketDataStock)
        {
            return new Stock
            {
                Symbol = marketDataStock.symbol,
                CompanyName = marketDataStock.companyName,
                Price = (decimal)marketDataStock.price,
                LastDiv = (decimal)marketDataStock.lastDiv,
                Industry = marketDataStock.industry,
                MarketCap = marketDataStock.mktCap
            };
        }
}