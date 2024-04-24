using Dokana.DTOs.Product;

namespace Dokana.DTOs
{
    public class StatisticsDto
    {
        public int NumberOfProducts { get; set; }

        public int NumberOfOrders { get; set; }

        public int NumberOfReviews { get; set; }

        public int NumberOfUsers { get; set; }

        public IEnumerable<ProductDto> TopSellingProducts { get; set; }

        public IEnumerable<UserDto> LastSignedUsers { get; set; }
    }
}