using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Dtos
{
    public class DealerCreateDto
    {
        [Required]
        [StringLength(50)]
        public string DealerCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string DealerName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DealerType { get; set; } = "Both";

        [StringLength(100)]
        public string ContactPerson { get; set; } = string.Empty;

        [StringLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
