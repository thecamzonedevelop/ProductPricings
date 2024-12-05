using Microsoft.EntityFrameworkCore;
using ProductLib;
using System.Text.Json;

public class SqlDbContext : DbContext
{
    public SqlDbContext(DbContextOptions<SqlDbContext> options)
        : base(options)
    {
    }

    // Override OnModelCreating if needed
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration<Product>(new ProductEntityTypeConfig());
        
        //Seeding products
        var products = ProductService.InitRequest.Select(req => req.ToEntity()).ToList();
        modelBuilder.Entity<Product>().HasData(products);
    }
}