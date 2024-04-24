using Dokana.DTOs.Product;

namespace Dokana.DTOs
{
    public class FavoriteDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public ProductDetailsDto ProductDetailsDto { get; set; }


        public string UserId { get; set; }
        public UserDto UserDto { get; set; }
    }
}