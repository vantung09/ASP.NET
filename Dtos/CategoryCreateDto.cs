using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Dtos
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(50)]
        public string CategoryCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
