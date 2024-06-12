using api.Data;
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
    public IActionResult GetAll()
    {
        _stockService.GetAllAsync();
        return Ok();
    }

    [HttpGet("id")]
    public IActionResult GetById()
    {
        _stockService.Get
    }

    [HttpPost]
    public IActionResult Create()
    {

    }

    [HttpPut]
    public IActionResult Update()
    {

    }

    [HttpDelete("id")]
    public IActionResult Delete()
    {

    }
}
