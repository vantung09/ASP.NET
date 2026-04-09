namespace ConnectDB.Dtos
{
    public class StockDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
