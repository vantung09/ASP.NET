using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;

namespace ConnectDB.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<WarehouseStock> WarehouseStocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WarehouseStock>()
                .HasKey(ws => new { ws.WarehouseId, ws.ProductId });

            modelBuilder.Entity<WarehouseStock>()
                .HasOne(ws => ws.Warehouse)
                .WithMany(w => w.WarehouseStocks)
                .HasForeignKey(ws => ws.WarehouseId);

            modelBuilder.Entity<WarehouseStock>()
                .HasOne(ws => ws.Product)
                .WithMany(p => p.WarehouseStocks)
                .HasForeignKey(ws => ws.ProductId);

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryCode)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductCode)
                .IsUnique();

            modelBuilder.Entity<Warehouse>()
                .HasIndex(w => w.WarehouseCode)
                .IsUnique();

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, CategoryCode = "CAT-FOOD", CategoryName = "Food", Description = "Food items" },
                new Category { Id = 2, CategoryCode = "CAT-TECH", CategoryName = "Technology", Description = "Tech items" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, ProductCode = "PRD-APPLE", ProductName = "Apple", Description = "Fresh apple", Unit = "kg", Price = 2.50m, CategoryId = 1 },
                new Product { Id = 2, ProductCode = "PRD-LAPTOP", ProductName = "Laptop", Description = "Office laptop", Unit = "pcs", Price = 899.00m, CategoryId = 2 }
            );

            // Warehouse and stock data can be added via API to avoid conflicts with existing data.
        }
    }
}
