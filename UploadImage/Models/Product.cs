using System.ComponentModel.DataAnnotations;

namespace UploadImage.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Price { get; set; }
        public string? Url { get; set; }
    }
}
