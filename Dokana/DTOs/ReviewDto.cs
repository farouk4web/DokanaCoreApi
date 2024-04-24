using System.ComponentModel.DataAnnotations;
using Dokana.DTOs.Product;

namespace Dokana.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public double StarsCount { get; set; }

        [Required, StringLength(500, MinimumLength = 3)]
        public string Comment { get; set; }

        public DateTime? DateOfCreate { get; set; }

        public int ProductId { get; set; }
        //public ProductDto ProductDto { get; set; }

        public string BuyerId { get; set; }
        public UserDto BuyerDto { get; set; }
    }
}