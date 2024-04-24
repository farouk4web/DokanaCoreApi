using System.ComponentModel.DataAnnotations;
using Dokana.DTOs.Product;

namespace Dokana.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public ProductDetailsDto ProductDetailsDto { get; set; }

        public int ShoppingCartId { get; set; }
        public ShoppingCartDto ShoppingCartDto { get; set; }
    }
}