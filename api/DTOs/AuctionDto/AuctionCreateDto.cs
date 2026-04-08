using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace api.DTOs.AuctionDto
{
    public class AuctionCreateDto
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public required string Category { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 1$")]
        public decimal Price { get; set; }
        [Required]
        public required DateTime EndAt { get; set; }

    }
}
