using System.ComponentModel.DataAnnotations;

namespace Dokana.Models
{
    public class Product
    {
        public int Id { get; set; }


        [Required, StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }

        [Required, StringLength(1500, MinimumLength = 200)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int UnitsInStore { get; set; }

        public bool AvailableToSale { get; set; }

        public bool ShowInSlider { get; set; }


        [Required]
        public string ImageSrc { get; set; }

        public int CountOfSale { get; set; }

        public double StarsCount { get; set; }



        // relatinships
        public int CategoryId { get; set; }
        public Category Category { get; set; }


        [Required]
        public string SellerId { get; set; }
        public ApplicationUser Seller { get; set; }



        public ICollection<Review> Reviews { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}