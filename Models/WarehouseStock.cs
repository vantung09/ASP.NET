using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class WarehouseStock
    {
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
