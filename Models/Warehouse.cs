using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string WarehouseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string WarehouseName { get; set; } = string.Empty;

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        public ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
    }
}
