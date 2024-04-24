﻿namespace Dokana.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int Quantity { get; set; }


        // relationships
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}