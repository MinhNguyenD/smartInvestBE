using portfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace portfolio.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> dbContextOptions) : base(dbContextOptions)
        {
        }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Holding> Holdings { get; set; }
        public DbSet<Analysis> Analyses {get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Many-to-Many relationship between Portfolio and Stock
            builder.Entity<Holding>()
                .HasKey(ps => new { ps.PortfolioId, ps.StockId });

            builder.Entity<Holding>()
                .HasOne(ps => ps.Portfolio)
                .WithMany(p => p.Holdings)
                .HasForeignKey(ps => ps.PortfolioId);

            builder.Entity<Holding>()
                .HasOne(ps => ps.Stock)
                .WithMany(s => s.Holdings)
                .HasForeignKey(ps => ps.StockId);

                // Many-To-One relationship between Analysis and Stock
            builder.Entity<Stock>()
                   .HasMany(a => a.Analyses)
                   .WithOne(s => s.Stock)
                   .HasForeignKey(p => p.StockId);
        }    
    }
}