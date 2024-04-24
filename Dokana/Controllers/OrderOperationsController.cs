using Dokana.Models;
using Dokana.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dokana.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class OrderOperationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OrderOperationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("ConfirmOrder/{id}")]
        [Authorize(Roles = RoleName.OwnersAndAdminsAndSellers)]
        public IActionResult ConfirmOrder(int id)
        {
            var orderInDb = _context.Orders.Find(id);
            if (orderInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");

            if (orderInDb.PaymentMethodId is not null)
            {
                orderInDb.IsConfirmed = true;
                orderInDb.DateOfConfirmation = DateTime.UtcNow;

                _context.SaveChanges();

                return Ok("Order is Confirmed successfully");
            }

            return BadRequest("this Order is not checkout yet");
        }

        [HttpPost("ShippingOrder/{id}")]
        [Authorize(Roles = RoleName.OwnersAndAdminsAndShippingStaff)]
        public IActionResult ShippingOrder(int id)
        {
            var orderInDb = _context.Orders.Find(id);
            if (orderInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");

            if (orderInDb.IsConfirmed)
            {
                orderInDb.IsShipping = true;
                orderInDb.DateOfShipping = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Order is Shipping successfully");
            }

            return BadRequest("something went wrong");
        }

        [HttpPost("DeliveredOrder/{id}")]
        [Authorize(Roles = RoleName.OwnersAndAdminsAndShippingStaff)]
        public IActionResult DeliveredOrder(int id)
        {
            var orderInDb = _context.Orders.Find(id);
            if (orderInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");

            if (orderInDb.IsShipping)
            {
                orderInDb.IsDelivered = true;
                orderInDb.DateOfDelivery = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Order Is Delivered successfully");
            }

            return BadRequest("something went wrong");
        }
    }
}
