using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStore.Models
{
    public class ProductDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required, MaxLength(100)]
        public string Brand { get; set; } = "";

        [Required, MaxLength(100)]
        public string Category { get; set; } = "";

        [Required]
        public Decimal Price { get; set; }

       
        public IFormFile? ImageFileName { get; set; } 
    }
}
