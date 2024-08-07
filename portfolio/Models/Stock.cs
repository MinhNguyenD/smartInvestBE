﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portfolio.Models;

[Table("stocks")]
public class Stock
{
    [Key]
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price {get; set;}

    [Column(TypeName = "decimal(18,2)")]
    public decimal LastDiv {get; set;}
    public string Industry { get; set; } = string.Empty;
    public long MarketCap { get; set; }
    public ICollection<Holding> Holdings { get; set; } = new List<Holding>();
    public ICollection<Analysis> Analyses { get; set; } = new List<Analysis>();
}
