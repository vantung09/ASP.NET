using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;

namespace ConnectDB.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<WarehouseStock> WarehouseStocks { get; set; }
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

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

            modelBuilder.Entity<Dealer>()
                .HasIndex(d => d.DealerCode)
                .IsUnique();

            modelBuilder.Entity<InventoryTransaction>()
                .HasIndex(t => t.TransactionCode)
                .IsUnique();

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(t => t.Warehouse)
                .WithMany(w => w.InventoryTransactions)
                .HasForeignKey(t => t.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(t => t.Product)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(t => t.Dealer)
                .WithMany(d => d.InventoryTransactions)
                .HasForeignKey(t => t.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, CategoryCode = "CAT-IPHONE", CategoryName = "iPhone", Description = "Dien thoai Apple iPhone" },
                new Category { Id = 2, CategoryCode = "CAT-MAC", CategoryName = "Mac", Description = "May tinh MacBook va iMac" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    ProductCode = "IP15PM-256",
                    ProductName = "iPhone 15 Pro Max 256GB",
                    Description = "Dien thoai cao cap, chip manh, camera dep.",
                    Brand = "Apple",
                    ImageUrl = "https://images.unsplash.com/photo-1695048133142-1a20484d2569?q=80&w=1200&auto=format&fit=crop",
                    Unit = "pcs",
                    Price = 31990000m,
                    OriginalPrice = 34990000m,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    ProductCode = "MBA-M3-13",
                    ProductName = "MacBook Air M3 13 inch",
                    Description = "Laptop mong nhe, pin lau, phu hop hoc tap va lam viec.",
                    Brand = "Apple",
                    ImageUrl = "https://images.unsplash.com/photo-1517336714739-489689fd1ca8?q=80&w=1200&auto=format&fit=crop",
                    Unit = "pcs",
                    Price = 27990000m,
                    OriginalPrice = 29990000m,
                    CategoryId = 2
                }
            );

            // Warehouse and stock data can be added via API to avoid conflicts with existing data.
        }
    }
}
