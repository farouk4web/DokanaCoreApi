using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }

        [Required, StringLength(60, MinimumLength = 4), RegularExpression("^[a-zA-Zء-ي ]*$")]
        public string FullName { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        // Address and 

        [Required]
        public string Country { get; set; }

        [Required]
        public string Region { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Street { get; set; }

        [StringLength(500, MinimumLength = 3)]
        public string MoreAboutAddress { get; set; }


        // Total Amount 
        public decimal? Total { get; set; }

        public decimal? ShippingFee { get; set; }

        public decimal? GrandTotal { get; set; }


        public int? PaymentMethodId { get; set; }
        public decimal? PaymentMethodFee { get; set; }


        // Levels
        public bool IsConfirmed { get; set; }
        public DateTime? DateOfConfirmation { get; set; }

        public bool IsShipping { get; set; }
        public DateTime? DateOfShipping { get; set; }

        public bool IsDelivered { get; set; }
        public DateTime? DateOfDelivery { get; set; }

        public DateTime? DateOfCreate { get; set; }

        // relationships
        public string BuyerId { get; set; }
        public UserDto BuyerDto { get; set; }

        public int ShoppingCartId { get; set; }
        public ShoppingCartDto ShoppingCart { get; set; }
    }
}