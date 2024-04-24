using Dokana.DTOs;
using Dokana.DTOs.Product;
using Dokana.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dokana.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShoppingCart
        [HttpGet]
        public IActionResult GetAll()
        {
            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            var currentUserShoppingCart = _context.ShoppingCarts
                                                        .Include(shoppingCart => shoppingCart.CartItems)
                                                        .ThenInclude(cartItem => cartItem.Product)
                                                        .SingleOrDefault(c => c.BuyerId == currentUserId && c.IdOfOrder == null);

            if (currentUserShoppingCart is null)
            {
                currentUserShoppingCart = new ShoppingCart
                {
                    BuyerId = currentUserId,
                    CartItems = new List<CartItem>()
                };

                _context.ShoppingCarts.Add(currentUserShoppingCart);
                _context.SaveChanges();
            }

            // populate Dto To send it To user
            var dto = new ShoppingCartDto
            {
                Id = currentUserShoppingCart.Id,
                IdOfOrder = currentUserShoppingCart.IdOfOrder,
                BuyerId = currentUserShoppingCart.BuyerId,

                CartItemsDto = currentUserShoppingCart.CartItems.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    Quantity = i.Quantity,
                    ShoppingCartId = i.ShoppingCartId,
                    ProductId = i.ProductId,
                    ProductDetailsDto = new ProductDetailsDto
                    {
                        Id = i.Product.Id,
                        Name = i.Product.Name,
                        Description = i.Product.Description,
                        ImageSrc = i.Product.ImageSrc,
                        Price = i.Product.Price,
                        AvailableToSale = i.Product.AvailableToSale,
                        UnitsInStore = i.Product.UnitsInStore,
                        StarsCount = i.Product.StarsCount,
                        CountOfSale = i.Product.CountOfSale,
                        ShowInSlider = i.Product.ShowInSlider,

                        CategoryId = i.Product.CategoryId,
                        SellerId = i.Product.SellerId
                    }
                })
            };

            return Ok(dto);
        }


        // ADD new item to the shopping cart
        [HttpPost]
        public IActionResult AddNewItem(NewCartItemDto dto)
        {
            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            var productInDb = _context.Products.Find(dto.ProductId);

            if (productInDb is null)
                return BadRequest("product Id Is Not Valid");

            // check if this product are available to sale or not
            if (!productInDb.AvailableToSale || productInDb.UnitsInStore == 0)
                return BadRequest("you cant add this product to your shopping cart at now");

            // check on quantity
            if (dto.Quantity > productInDb.UnitsInStore)
                dto.Quantity = productInDb.UnitsInStore;


            // get shopping cart with null order id and Create one if there is no cart available
            var currentUserShppingCart = _context.ShoppingCarts.SingleOrDefault(c => c.BuyerId == currentUserId && c.IdOfOrder == null);
            if (currentUserShppingCart is null)
            {
                currentUserShppingCart = new ShoppingCart
                {
                    BuyerId = currentUserId,
                    CartItems = new List<CartItem>()
                };

                _context.ShoppingCarts.Add(currentUserShppingCart);
                _context.SaveChanges();
            }

            // check if this product added before, or not
            var itemInDb = _context.CartItems.SingleOrDefault(c => c.ProductId == productInDb.Id && c.ShoppingCartId == currentUserShppingCart.Id);

            if (itemInDb is null)
            {
                // create cart item to add it to user shopping cart
                itemInDb = new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    ShoppingCartId = currentUserShppingCart.Id
                };

                _context.CartItems.Add(itemInDb);
            }
            else
                itemInDb.Quantity = dto.Quantity;

            _context.SaveChanges();


            // populate dto by item data then send it
            var dtoDetaild = new CartItemDto
            {
                Id = itemInDb.Id,
                Quantity = itemInDb.Quantity,
                ShoppingCartId = itemInDb.ShoppingCartId,
                ProductId = itemInDb.ProductId,
                ProductDetailsDto = new ProductDetailsDto
                {
                    Id = productInDb.Id,
                    Name = productInDb.Name,
                    Description = productInDb.Description,
                    ImageSrc = productInDb.ImageSrc,
                    Price = productInDb.Price,
                    CountOfSale = productInDb.CountOfSale,
                    AvailableToSale = productInDb.AvailableToSale,
                    UnitsInStore = productInDb.UnitsInStore,
                    StarsCount = productInDb.StarsCount,
                    ShowInSlider = productInDb.ShowInSlider,
                    CategoryId = productInDb.CategoryId,
                    SellerId = productInDb.SellerId
                }
            };

            return Ok(dtoDetaild);
        }


        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            var item = _context.CartItems
                                    .Include(i => i.ShoppingCart)
                                    .SingleOrDefault(i => i.Id == id);

            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            if (item is null || item.ShoppingCart.IdOfOrder is not null || item.ShoppingCart.BuyerId != currentUserId)
                return NotFound("sorry we dont found what you are looking for ):");

            _context.CartItems.Remove(item);
            _context.SaveChanges();

            return Ok("Your product Removed successfully from your cart");
        }
    }
}
