using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectDB.Models
{
    public class InventoryTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string TransactionType { get; set; } = "Import";

        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int DealerId { get; set; }
        public Dealer? Dealer { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string Note { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
