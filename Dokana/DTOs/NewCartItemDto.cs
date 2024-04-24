using System.ComponentModel.DataAnnotations;
using Dokana.DTOs.Product;

namespace Dokana.DTOs
{
    public class NewCartItemDto
    {
        public int Quantity { get; set; }

        public int ProductId { get; set; }
    }
}