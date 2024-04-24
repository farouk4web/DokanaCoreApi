using Dokana.DTOs;
using Dokana.DTOs.Product;
using Dokana.Models;
using Dokana.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dokana.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AboutController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // get site statistics
        [HttpGet("Statistics")]
        public IActionResult Statistics()
        {
            var lastSignedUsers = _context.Users.OrderByDescending(u => u.JoinDate).Take(4);
            var topSellingProducts = _context.Products.OrderByDescending(p => p.CountOfSale).Take(3);

            var dto = new StatisticsDto
            {
                NumberOfProducts = _context.Products.Count(),
                NumberOfOrders = _context.Orders.Count(),
                NumberOfReviews = _context.Reviews.Count(),
                NumberOfUsers = _context.Users.Count(),

                LastSignedUsers = lastSignedUsers.Select(u => new UserDto
                {
                    Id = u.Id,
                    JoinDate = u.JoinDate,
                    FullName = u.FullName,
                    Email = u.Email,
                    ProfileImageSrc = u.ProfileImageSrc,
                }),

                TopSellingProducts = topSellingProducts.Select(p => new ProductDetailsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageSrc = p.ImageSrc,
                    Price = p.Price,
                    Description = p.Description,
                    AvailableToSale = p.AvailableToSale,
                    CountOfSale = p.CountOfSale,
                    UnitsInStore = p.UnitsInStore,
                    ShowInSlider = p.ShowInSlider,
                    StarsCount = p.StarsCount,
                    CategoryId = p.CategoryId,
                    SellerId = p.SellerId
                })
            };

            return Ok(dto);
        }

        [HttpGet("SiteSettings")]
        [AllowAnonymous]
        public IActionResult GetSiteSettings()
        {
            var dto = new SiteSettingsDto
            {
                CurrencySign = _configuration.GetValue<string>("AppSetting:CurrencySign"),
                ShippingFee = _configuration.GetValue<decimal>("AppSetting:ShippingFee"),
                CashOnDelivaryFee = _configuration.GetValue<decimal>("AppSetting:CashOnDelivaryFee")
            };

            return Ok(dto);
        }
    }
}
