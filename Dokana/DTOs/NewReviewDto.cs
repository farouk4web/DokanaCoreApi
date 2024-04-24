using System.ComponentModel.DataAnnotations;
using Dokana.DTOs.Product;

namespace Dokana.DTOs
{
    public class NewReviewDto
    {
        [Range(1, 5)]
        public double StarsCount { get; set; }

        [Required, StringLength(500, MinimumLength = 3)]
        public string Comment { get; set; }

        public int ProductId { get; set; }
    }
}