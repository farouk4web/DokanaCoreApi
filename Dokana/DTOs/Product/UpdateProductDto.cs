using Dokana.Settings;
using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs.Product
{
    public class UpdateProductDto : ProductDto
    {
        [SupportedExtentions]
        [GenralSize]
        public IFormFile Picture { get; set; }
    }
}
