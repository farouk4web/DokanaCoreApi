
using Dokana.Settings;
using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs.Product
{
    public class NewProductDto : ProductDto
    {
        [Required(ErrorMessage = "You should choose photo for this product")]
        [SupportedExtentions]
        [GenralSize]
        public IFormFile Picture { get; set; }
    }
}
