using Dokana.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Dokana.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(200, MinimumLength = 3), RegularExpression("^[a-zA-Zء-ي ]*$")]
        public string Name { get; set; }

        [Required, StringLength(700, MinimumLength = 200)]
        public string Description { get; set; }
    }
}