using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs.Product
{
    public class ProductDetailsDto : ProductDto
    {
        public int Id { get; set; }

        public string ImageSrc { get; set; }

        public int CountOfSale { get; set; }

        public double StarsCount { get; set; }

        public CategoryDto CategoryDto { get; set; }

        public string SellerId { get; set; }

        public UserDto SellerDto { get; set; }
    }
}