using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Dtos
{
    public class StockUpsertDto
    {
        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
