using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs.Order
{
    public class NewOrderDto
    {
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
    }
}
