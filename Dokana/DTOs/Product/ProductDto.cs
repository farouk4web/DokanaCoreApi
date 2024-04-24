using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs.Product
{
    public class ProductDto
    {
        [Required, StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }

        [Required, MinLength(250)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int UnitsInStore { get; set; }

        public bool AvailableToSale { get; set; }

        public bool ShowInSlider { get; set; }

        public int CategoryId { get; set; }
    }
}