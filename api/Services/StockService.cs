using api.Data;
using Microsoft.EntityFrameworkCore;

namespace api;

public class StockService
{
    private readonly ApplicationDBContext _dbContext;
    public StockService(ApplicationDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Stock>> GetAllAsync()
    {
        return await _dbContext.Stocks.ToListAsync();
    }

    public async Task<Stock?> GetByIdAsync(long id)
    {
        var stock = await _dbContext.Stocks.FindAsync(id);
        return stock;
    }

    public async Task<Stock> CreateAsync(Stock stock)
    {
        await _dbContext.Stocks.AddAsync(stock);
        await _dbContext.SaveChangesAsync();
        return stock;
    }

    public async Task<Stock?> UpdateAsync(long id, UpdateStockDto updateStockDto)
    {
        var stock = await _dbContext.Stocks.FindAsync(id);
        if (stock == null)
        {
            return null;
        }
        // mapper here
        return stock;
    }
    public async Task DeleteAsync(long id)
    {
        await _dbContext.Stocks.Where(stock => stock.Id == id).ExecuteDeleteAsync();
    }
}
