namespace Dokana.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public int? IdOfOrder { get; set; }
        //public Order Order { get; set; }

        public string BuyerId { get; set; }
        public ApplicationUser Buyer { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
    }
}