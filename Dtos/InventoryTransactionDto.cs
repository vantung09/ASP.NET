namespace ConnectDB.Dtos
{
    public class InventoryTransactionDto
    {
        public int Id { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;

        public int WarehouseId { get; set; }
        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        public int DealerId { get; set; }
        public string DealerCode { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
