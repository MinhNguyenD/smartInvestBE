using api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace api;


[ApiController]
[Route("api/stock")]
public class StockController : ControllerBase
{
    private readonly StockService _stockService;
    public StockController(StockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stocks = await _stockService.GetAllAsync();
        return Ok(stocks);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var stock = await _stockService.GetByIdAsync(id);
        return Ok(stock);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStockDto createStockDto)
    {
        await _stockService.CreateAsync(StockMapper.ToStockFromCreateDTO(createStockDto));
        var stockModel = createStockDto.ToStockFromCreateDTO();
        return CreatedAtAction(nameof(GetById), new { id = stockModel.Id}, StockMapper.ToStockDto(stockModel));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateStockDto updateStockDto)
    {   
        await _stockService.UpdateAsync(id, updateStockDto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _stockService.DeleteAsync(id);
        return NoContent();
    }
}
