using Microsoft.EntityFrameworkCore;
using portfolio.Data;
using portfolio.Models;

namespace portfolio.Services;

public interface IPortfolioService
{
    Task<List<Stock>> GetHoldings(string userId);
    Task AddStockAsync(string userId, string symbol);
    Task RemoveStockAsync(string userId, string symbol);
    Task<Portfolio> CreatePortfolio(string userId);
}

public class PortfolioService : IPortfolioService
{
    private readonly ApplicationDBContext _dbContext;
    private readonly IMarketDataService _marketDataService;

    public PortfolioService(ApplicationDBContext dbContext, IMarketDataService marketDataService)
    {
        _dbContext = dbContext;
        _marketDataService = marketDataService;
    }

    public async Task<List<Stock>> GetHoldings(string userId){
        return await _dbContext.Portfolios.Where(p => p.UserId == userId).SelectMany(p => p.Holdings).Select(h => h.Stock).ToListAsync();
    } 

    public async Task AddStockAsync(string userId, string symbol){
        // get current user portfolio
        var portfolio = await _dbContext.Portfolios.Include(p => p.Holdings).ThenInclude(ps => ps.Stock).FirstOrDefaultAsync(p => p.UserId == userId);

        // check if stock in db
        var stock = await _dbContext.Stocks.FirstOrDefaultAsync(s => s.Symbol.Equals(symbol));
        if(stock == null){
            stock = await _marketDataService.FindStockBySymbolAsync(symbol);
            if(stock == null) {
                throw new Exception("Stock not found");
            }
            else {
                await _dbContext.Stocks.AddAsync(stock);
                await _dbContext.SaveChangesAsync();
            }
        }       
        
        if(portfolio == null){
            portfolio = await CreatePortfolio(userId);
        }
        else {
            // check if added stock already in portfolio
            if(portfolio!.Holdings.Any(h => h.Stock.Symbol.ToLower().Equals(symbol.ToLower()))){
                throw new Exception("Stock is already saved");
            }
        }
       
 
        var holding = new Holding{
            StockId = stock.Id,
            PortfolioId = portfolio!.Id
        };

        await _dbContext.Holdings.AddAsync(holding);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveStockAsync(string userId, string symbol){
        // get current user portfolio
        var portfolio = await _dbContext.Portfolios.Include(p => p.Holdings).ThenInclude(ps => ps.Stock).FirstOrDefaultAsync(p => p.UserId == userId);

        if(portfolio == null){
            throw new Exception("User Portfolio not found");
        }

        var removeStock = portfolio.Holdings.FirstOrDefault(h => h.Stock.Symbol.ToLower().Equals(symbol.ToLower()));

        if(removeStock == null){
            return;
        }

        _dbContext.Holdings.Remove(removeStock);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Portfolio> CreatePortfolio(string userId){
        var portfolio = new Portfolio{
            UserId = userId,
            Holdings = []
        };

        await _dbContext.Portfolios.AddAsync(portfolio);
        await _dbContext.SaveChangesAsync();
        return portfolio;
    }
}

