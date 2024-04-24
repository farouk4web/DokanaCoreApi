using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs
{
    public class SiteSettingsDto
    {
        [Required]
        public string CurrencySign { get; set; }

        [Required, Range(0, 10000)]
        public decimal ShippingFee { get; set; }

        [Required, Range(0, 10000)]
        public decimal CashOnDelivaryFee { get; set; }
    }
}