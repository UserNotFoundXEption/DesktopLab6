using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Shared;

namespace API.Models;

public class ProductsDbContext : DbContext
{
    public DbSet<Product> products { get; set; }

    private readonly IConfiguration _configuration;


    public ProductsDbContext(DbContextOptions<ProductsDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
                .HasKey(p => p.Name);

        modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .IsRequired()
                .HasColumnType("float");

        modelBuilder.Entity<Product>()
                .Property(p => p.UnitsAvailable)
                .IsRequired()
                .HasColumnType("int");
    }
}

