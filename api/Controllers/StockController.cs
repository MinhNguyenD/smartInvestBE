using api.Data;
using Microsoft.AspNetCore.Mvc;

namespace api;


[ApiController]
[Route("api/stock")]
public class StockController : ControllerBase
{
    private readonly ApplicationDBContext _dbContext;
    public StockController(ApplicationDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult GetAll(){

    }

    [HttpGet({"id"})]
    public IActionResult GetById(){

    }

    [HttpPost]
    public IActionResult Create(){

    }

    [HttpPut]
    public IActionResult Update(){

    }

    [HttpDelete({"id"})]
    public IActionResult Delete(){

    }
}
