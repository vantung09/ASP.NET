namespace ConnectDB.Dtos
{
    public class DealerDto
    {
        public int Id { get; set; }
        public string DealerCode { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;
        public string DealerType { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
