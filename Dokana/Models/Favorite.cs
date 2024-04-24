using System.ComponentModel.DataAnnotations;

namespace Dokana.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }


        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}