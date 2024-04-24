using Dokana.DTOs;
using Dokana.Models;
using Dokana.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dokana.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public CheckoutController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Checkout(CheckoutDto dto)
        {
            var orderInDb = _context.Orders
                                        .Include(o => o.ShoppingCart)
                                        .ThenInclude(cart => cart.CartItems)
                                        .ThenInclude(item => item.Product)
                                        .SingleOrDefault(o => o.Id == dto.OrderId);

            if (orderInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");
            if (orderInDb.PaymentMethodId is not null)
                return BadRequest("you are actully Checkout before");



            // now pay using choosed payment method
            var operationResult = false;

            if (dto.PaymentMethodId == PaymentMethodsIDs.CashOnDelivaryId)
            {
                operationResult = true;
            }

            else if (dto.PaymentMethodId == PaymentMethodsIDs.PaypalId)
            {
                // Paypal Code

                operationResult = true;
            }

            else if (dto.PaymentMethodId == PaymentMethodsIDs.VisaCardId)
            {
                // visacard Code

                operationResult = true;
            }

            else if (dto.PaymentMethodId == PaymentMethodsIDs.VodafoneCashId)
            {
                // vodafone cash api Code here;

                operationResult = true;
            }

            else
                return BadRequest("you should choose one of pyment methods are supported like {1 => CashOnDelivary, 2 => Paypal, 3 => VisaCard, 4 => VodafoneCash}");


            if (!operationResult)
                return BadRequest("some thing went wrong please try again later");


            // update order and add payment method fee
            orderInDb.PaymentMethodId = PaymentMethodsIDs.CashOnDelivaryId;

            orderInDb.PaymentMethodFee = orderInDb.PaymentMethodId == PaymentMethodsIDs.CashOnDelivaryId ? _configuration.GetValue<decimal>("CashOnDelivaryFee") : 0;
            orderInDb.GrandTotal += orderInDb.PaymentMethodFee;

            // now remove the quantity from every product in unit in store field
            orderInDb.ShoppingCart.CartItems.ToList().ForEach(item => item.Product.UnitsInStore = item.Product.UnitsInStore - item.Quantity);
            orderInDb.ShoppingCart.CartItems.ToList().ForEach(item => item.Product.CountOfSale = item.Product.CountOfSale + item.Quantity);
            _context.SaveChanges();

            return Ok("Your Order Checkout Successfuly, wait Untel or sellers Confirm it");
        }
    }
}
