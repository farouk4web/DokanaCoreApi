using Dokana.DTOs;
using Dokana.DTOs.Order;
using Dokana.DTOs.Product;
using Dokana.Models;
using Dokana.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dokana.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public OrdersController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = RoleName.OwnersAndAdminsAndSellersAndShippingStaff)]
        public IActionResult AllOrders(string level = "all", int pageNumber = 1)
        {
            int elementInPage = 10;
            var groupOfOrders = new List<Order>();
            switch (level)
            {
                case "notPaid":
                    groupOfOrders = _context.Orders
                                    .Where(o => o.PaymentMethodId == null)
                                    .OrderByDescending(o => o.Id)
                                    .Skip((pageNumber - 1) * elementInPage)
                                    .Take(elementInPage)
                                    .ToList();
                    break;

                case "paid":
                    groupOfOrders = _context.Orders
                                    .Where(o => o.PaymentMethodId != null && o.IsConfirmed == false)
                                    .OrderByDescending(o => o.Id)
                                    .Skip((pageNumber - 1) * elementInPage)
                                    .Take(elementInPage)
                                    .ToList();
                    break;

                case "confirmed":
                    groupOfOrders = _context.Orders
                                    .Where(o => o.IsConfirmed && o.IsShipping == false)
                                    .OrderByDescending(o => o.Id)
                                    .Skip((pageNumber - 1) * elementInPage)
                                    .Take(elementInPage)
                                    .ToList();
                    break;

                case "shipping":
                    groupOfOrders = _context.Orders
                                    .Where(o => o.IsShipping && o.IsDelivered == false)
                                    .OrderByDescending(o => o.Id)
                                    .Skip((pageNumber - 1) * elementInPage)
                                    .Take(elementInPage)
                                    .ToList();
                    break;

                case "delivered":
                    groupOfOrders = _context.Orders
                                    .Where(o => o.IsDelivered)
                                    .OrderByDescending(o => o.Id)
                                    .Skip((pageNumber - 1) * elementInPage)
                                    .Take(elementInPage)
                                    .ToList();
                    break;

                default:
                    groupOfOrders = _context.Orders
                                            .OrderByDescending(o => o.Id)
                                            .Skip((pageNumber - 1) * elementInPage)
                                            .Take(elementInPage)
                                            .ToList();
                    break;
            }

            var ordersDto = groupOfOrders.Select(o => new OrderDto
            {
                Id = o.Id,
                BuyerId = o.BuyerId,
                ShoppingCartId = o.ShoppingCartId,

                FullName = o.FullName,
                Phone = o.Phone,

                City = o.City,
                Country = o.Country,
                Region = o.Region,
                Street = o.Street,
                MoreAboutAddress = o.MoreAboutAddress,

                PaymentMethodFee = o.PaymentMethodFee,
                PaymentMethodId = o.PaymentMethodId,

                Total = o.Total,
                ShippingFee = o.ShippingFee,
                GrandTotal = o.GrandTotal,

                DateOfCreate = o.DateOfCreate,
                DateOfConfirmation = o.DateOfConfirmation,
                DateOfDelivery = o.DateOfDelivery,
                DateOfShipping = o.DateOfShipping,

                IsConfirmed = o.IsConfirmed,
                IsDelivered = o.IsDelivered,
                IsShipping = o.IsShipping
            });

            return Ok(ordersDto);
        }


        [HttpGet("UserOrders")]
        public IActionResult UserOrders()
        {
            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            var userOrders = _context.Orders
                                    .Where(o => o.BuyerId == currentUserId)
                                    .OrderByDescending(o => o.Id)
                                    .ToList();

            var ordersDto = userOrders.Select(o => new OrderDto
            {
                Id = o.Id,
                BuyerId = o.BuyerId,
                ShoppingCartId = o.ShoppingCartId,

                FullName = o.FullName,
                Phone = o.Phone,

                City = o.City,
                Country = o.Country,
                Region = o.Region,
                Street = o.Street,
                MoreAboutAddress = o.MoreAboutAddress,

                PaymentMethodFee = o.PaymentMethodFee,
                PaymentMethodId = o.PaymentMethodId,

                Total = o.Total,
                ShippingFee = o.ShippingFee,
                GrandTotal = o.GrandTotal,

                DateOfCreate = o.DateOfCreate,
                DateOfConfirmation = o.DateOfConfirmation,
                DateOfDelivery = o.DateOfDelivery,
                DateOfShipping = o.DateOfShipping,

                IsConfirmed = o.IsConfirmed,
                IsDelivered = o.IsDelivered,
                IsShipping = o.IsShipping
            });

            return Ok(ordersDto);
        }


        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var orderInDb = _context.Orders
                                    .Include(o => o.ShoppingCart)
                                    .ThenInclude(cart => cart.CartItems)
                                    .ThenInclude(item => item.Product)
                                    .Include(o => o.Buyer)
                                    .SingleOrDefault(o => o.Id == id);

            if (orderInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");

            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            if (orderInDb.BuyerId == currentUserId || User.IsInRole(RoleName.Owners) || User.IsInRole(RoleName.Admins)
                || User.IsInRole(RoleName.Sellers) || User.IsInRole(RoleName.ShippingStaff))
            {
                var dto = new OrderDto
                {
                    Id = orderInDb.Id,

                    FullName = orderInDb.FullName,
                    Phone = orderInDb.Phone,

                    City = orderInDb.City,
                    Country = orderInDb.Country,
                    Region = orderInDb.Region,
                    Street = orderInDb.Street,
                    MoreAboutAddress = orderInDb.MoreAboutAddress,

                    PaymentMethodFee = orderInDb.PaymentMethodFee,
                    PaymentMethodId = orderInDb.PaymentMethodId,

                    Total = orderInDb.Total,
                    ShippingFee = orderInDb.ShippingFee,
                    GrandTotal = orderInDb.GrandTotal,

                    DateOfCreate = orderInDb.DateOfCreate,
                    DateOfConfirmation = orderInDb.DateOfConfirmation,
                    DateOfDelivery = orderInDb.DateOfDelivery,
                    DateOfShipping = orderInDb.DateOfShipping,

                    IsConfirmed = orderInDb.IsConfirmed,
                    IsDelivered = orderInDb.IsDelivered,
                    IsShipping = orderInDb.IsShipping,

                    BuyerId = orderInDb.BuyerId,
                    ShoppingCartId = orderInDb.ShoppingCartId,

                    ShoppingCart = new ShoppingCartDto
                    {
                        Id = orderInDb.ShoppingCart.Id,
                        IdOfOrder = orderInDb.ShoppingCart.IdOfOrder,
                        BuyerId = orderInDb.ShoppingCart.BuyerId,
                        CartItemsDto = orderInDb.ShoppingCart.CartItems.Select(i => new CartItemDto
                        {
                            Id = i.Id,
                            Quantity = i.Quantity,
                            ProductDetailsDto = new ProductDetailsDto
                            {
                                Id = i.Product.Id,
                                Name = i.Product.Name,
                                ImageSrc = i.Product.ImageSrc,
                                Price = i.Product.Price,
                                Description = i.Product.Description,
                                AvailableToSale = i.Product.AvailableToSale,
                                UnitsInStore = i.Product.UnitsInStore,
                                CountOfSale = i.Product.CountOfSale,
                                StarsCount = i.Product.StarsCount,
                                ShowInSlider = i.Product.ShowInSlider,
                                CategoryId = i.Product.CategoryId,
                                SellerId = i.Product.SellerId
                            }
                        })
                    },
                    BuyerDto = new UserDto
                    {
                        Id = orderInDb.Buyer.Id,
                        FullName = orderInDb.Buyer.FullName,
                        Email = orderInDb.Buyer.Email,
                        JoinDate = orderInDb.Buyer.JoinDate,
                        ProfileImageSrc = orderInDb.Buyer.ProfileImageSrc
                    }
                };

                return Ok(dto);
            }

            return BadRequest("Something went wrong");
        }


        [HttpPost]
        public IActionResult Create(NewOrderDto newOrderDto)
        {
            var shippingFee = Convert.ToDecimal(_configuration.GetValue<string>("shippingFee"));
            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");

            var currentUserShoppingCart = _context.ShoppingCarts
                                                    .Include(m => m.CartItems)
                                                    .ThenInclude(item => item.Product)
                                                    .SingleOrDefault(i => i.BuyerId == currentUserId && i.IdOfOrder == null);

            if (currentUserShoppingCart is null || currentUserShoppingCart.CartItems.Count == 0)
                return BadRequest("you dont have any product in your Cart");


            var notAllowedCartItems = new List<CartItem>();
            foreach (var item in currentUserShoppingCart.CartItems)
            {
                if (!item.Product.AvailableToSale || item.Product.UnitsInStore == 0)
                {
                    notAllowedCartItems.Add(item);
                }
            }

            // create new shopping cart and populate it with products { NotAvailable & No Units in store }
            // Now populate it in another shopping cart
            if (notAllowedCartItems.Count is not 0)
            {
                var newShoppingCart4CurrentUser = new ShoppingCart
                {
                    BuyerId = currentUserId
                };

                _context.ShoppingCarts.Add(newShoppingCart4CurrentUser);
                _context.SaveChanges();

                notAllowedCartItems.ForEach(m => m.ShoppingCartId = newShoppingCart4CurrentUser.Id);
                _context.SaveChanges();
            }


            if (currentUserShoppingCart.CartItems.Count is 0)
                return BadRequest("All Products you are request is not allowed to pay now ):");

            // add new order to database
            decimal total = 0;
            foreach (var item in currentUserShoppingCart.CartItems)
            {
                total += item.Quantity * item.Product.Price;
            }

            var newOrder = new Order
            {
                // main data 
                FullName = newOrderDto.FullName,
                Phone = newOrderDto.Phone,

                // address
                Country = newOrderDto.Country,
                Region = newOrderDto.Region,
                City = newOrderDto.City,
                Street = newOrderDto.Street,
                MoreAboutAddress = newOrderDto.MoreAboutAddress,

                Total = total,
                ShippingFee = shippingFee,
                GrandTotal = total + shippingFee, // without payment method fee                    

                // relate with current user
                BuyerId = currentUserId,
                ShoppingCartId = currentUserShoppingCart.Id,

                // add utc date Time 
                DateOfCreate = DateTime.UtcNow
            };

            // insert the order to database
            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            // relate shopingCart with this order
            currentUserShoppingCart.IdOfOrder = newOrder.Id;
            _context.SaveChanges();

            // populate data and send it again
            var dto = new OrderDto
            {
                Id = newOrder.Id,
                FullName = newOrder.FullName,
                Phone = newOrder.Phone,
                Country = newOrder.Country,
                Region = newOrder.Region,
                City = newOrder.City,
                Street = newOrder.Street,
                ShoppingCartId = newOrder.ShoppingCartId,
                ShippingFee = newOrder.ShippingFee,
                Total = newOrder.Total,
                GrandTotal = newOrder.GrandTotal,
                DateOfCreate = newOrder.DateOfCreate,
                BuyerId = newOrder.BuyerId,
                MoreAboutAddress = newOrder.MoreAboutAddress,
            };

            return Ok(dto);
        }


        [HttpDelete("{id}")]
        public IActionResult Cancel(int id)
        {
            var orderInDB = _context.Orders
                                .Include(o => o.ShoppingCart)
                                .ThenInclude(cart => cart.CartItems)
                                .SingleOrDefault(o => o.Id == id);

            if (orderInDB is null)
                return NotFound("sorry we dont found what you are looking for ):");


            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            if (orderInDB.BuyerId == currentUserId || User.IsInRole(RoleName.Owners) || User.IsInRole(RoleName.Admins))
            {
                if (orderInDB.IsConfirmed)
                    return BadRequest("You cant Cancel Confirmed Order");

                _context.CartItems.RemoveRange(orderInDB.ShoppingCart.CartItems);
                _context.ShoppingCarts.Remove(orderInDB.ShoppingCart);
                _context.Orders.Remove(orderInDB);
                _context.SaveChanges();

                return Ok("Your Order Canceled Successfully");
            }

            return BadRequest("Something went wrong");
        }
    }
}
