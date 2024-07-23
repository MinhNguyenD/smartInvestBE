﻿using portfolio.Data;
using portfolio.Dtos;
using Microsoft.EntityFrameworkCore;
using portfolio.Models;

namespace portfolio.Services;

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

    public async Task<Stock?> GetByIdAsync(int id)
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

    public async Task<Stock?> UpdateAsync(int id, UpdateStockDto updateStockDto)
    {
        var stock = await _dbContext.Stocks.FindAsync(id);
        if (stock is null)
        {
            return null;
        }
        _dbContext.Entry(stock).CurrentValues.SetValues(updateStockDto);
        await _dbContext.SaveChangesAsync();
        return stock;
    }

    public async Task<Stock?> DeleteAsync(int id)
    {
        var stock = await _dbContext.Stocks.FindAsync(id);
        if (stock is null)
        {
            return null;
        }
        _dbContext.Stocks.Remove(stock);
        await _dbContext.SaveChangesAsync();
        return stock;
    }
}
