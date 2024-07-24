using System.ComponentModel.DataAnnotations.Schema;

namespace portfolio.Dtos;

public class UpdateStockDto
{
    public string Symbol { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal LastDiv { get; set; }
    public string Industry { get; set; } = string.Empty;
    public long MarketCap { get; set; }
}
