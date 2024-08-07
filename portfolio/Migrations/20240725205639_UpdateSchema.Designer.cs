﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using portfolio.Data;

#nullable disable

namespace portfolio.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20240725205639_UpdateSchema")]
    partial class UpdateSchema
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("portfolio.Models.Analysis", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("DateCreated")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("StockId")
                        .HasColumnType("int");

                    b.Property<string>("StockSymbol")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("StockId");

                    b.ToTable("Analyses");
                });

            modelBuilder.Entity("portfolio.Models.Holding", b =>
                {
                    b.Property<int>("PortfolioId")
                        .HasColumnType("int");

                    b.Property<int>("StockId")
                        .HasColumnType("int");

                    b.HasKey("PortfolioId", "StockId");

                    b.HasIndex("StockId");

                    b.ToTable("holdings");
                });

            modelBuilder.Entity("portfolio.Models.Portfolio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("portfolios");
                });

            modelBuilder.Entity("portfolio.Models.Stock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Industry")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("LastDiv")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long>("MarketCap")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("stocks");
                });

            modelBuilder.Entity("portfolio.Models.Analysis", b =>
                {
                    b.HasOne("portfolio.Models.Stock", "Stock")
                        .WithMany("Analyses")
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("portfolio.Models.Holding", b =>
                {
                    b.HasOne("portfolio.Models.Portfolio", "Portfolio")
                        .WithMany("Holdings")
                        .HasForeignKey("PortfolioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("portfolio.Models.Stock", "Stock")
                        .WithMany("Holdings")
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Portfolio");

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("portfolio.Models.Portfolio", b =>
                {
                    b.Navigation("Holdings");
                });

            modelBuilder.Entity("portfolio.Models.Stock", b =>
                {
                    b.Navigation("Analyses");

                    b.Navigation("Holdings");
                });
#pragma warning restore 612, 618
        }
    }
}
