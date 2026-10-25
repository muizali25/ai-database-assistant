using Microsoft.EntityFrameworkCore;
using MiniPosAi.Models;

namespace MiniPosAi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => x.CategoryId);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(250);
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.ProductId);
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(x => x.CustomerId);
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.Email).HasMaxLength(150).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(30);
        });

        modelBuilder.Entity<Sale>(e =>
        {
            e.HasKey(x => x.SaleId);
            e.Property(x => x.TotalAmount).HasPrecision(18, 2);
            e.HasOne(x => x.Customer)
                .WithMany(x => x.Sales)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SaleItem>(e =>
        {
            e.HasKey(x => x.SaleItemId);
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
            e.HasOne(x => x.Sale)
                .WithMany(x => x.SaleItems)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product)
                .WithMany(x => x.SaleItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        ApplySeedData(modelBuilder);
    }

    private static void ApplySeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(SeedData.Categories);
        modelBuilder.Entity<Customer>().HasData(SeedData.Customers);
        modelBuilder.Entity<Product>().HasData(SeedData.Products);
        modelBuilder.Entity<Sale>().HasData(SeedData.Sales);
        modelBuilder.Entity<SaleItem>().HasData(SeedData.SaleItems);
    }
}
