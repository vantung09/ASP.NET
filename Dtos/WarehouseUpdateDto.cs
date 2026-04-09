using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Dtos
{
    public class WarehouseUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string WarehouseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string WarehouseName { get; set; } = string.Empty;

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
    }
}
