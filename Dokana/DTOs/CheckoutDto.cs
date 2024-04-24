using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs
{
    public class CheckoutDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int PaymentMethodId { get; set; }
    }
}