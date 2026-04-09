namespace ConnectDB.Dtos
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
