using Dokana.DTOs;
using Dokana.DTOs.Product;
using Dokana.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dokana.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            var currentUserFavoritesItems = _context.Favorites
                                                        .Include(f => f.Product)
                                                        .ThenInclude(product => product.Category)
                                                        .Where(f => f.UserId == currentUserId)
                                                        .ToList();

            var favoritesDto = currentUserFavoritesItems.Select(c => new FavoriteDto
            {
                Id = c.Id,
                UserId = c.UserId,
                ProductId = c.ProductId,
                ProductDetailsDto = new ProductDetailsDto
                {
                    Id = c.Product.Id,
                    Name = c.Product.Name,
                    ImageSrc = c.Product.ImageSrc,
                    Price = c.Product.Price,
                    StarsCount = c.Product.StarsCount,
                    AvailableToSale = c.Product.AvailableToSale,
                    CountOfSale = c.Product.CountOfSale,
                    Description = c.Product.Description,
                    UnitsInStore = c.Product.UnitsInStore,
                    ShowInSlider = c.Product.ShowInSlider,
                    SellerId = c.Product.SellerId,
                    CategoryId = c.Product.CategoryId,
                    CategoryDto = new CategoryDto
                    {
                        Id = c.Product.Category.Id,
                        Name = c.Product.Category.Name,
                        Description = c.Product.Category.Description
                    }
                }
            });

            return Ok(favoritesDto);
        }

        [HttpPost]
        public IActionResult AddToMyFavorites(NewFavoriteDto dto)
        {
            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");

            // check if user add this product to his favorites before or not
            var isSignedBefore = _context.Favorites.Any(i => i.ProductId == dto.ProductId && i.UserId == currentUserId);
            if (isSignedBefore)
                return BadRequest("this product is actully in your favorites");

            var productInDB = _context.Products.Find(dto.ProductId);
            if (productInDB is null)
                return BadRequest("Are You Kidding me (^_^)");


            var item = new Favorite()
            {
                ProductId = dto.ProductId,
                UserId = currentUserId
            };

            _context.Favorites.Add(item);
            _context.SaveChanges();

            return Ok("Product added to Your Favorite Successfully");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFromFavorites(int id)
        {
            var item = _context.Favorites.Find(id);
            if (item is null)
                return NotFound("sorry we dont found what you are looking for ):");

            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            if (item.UserId != currentUserId)
                return BadRequest("It is Not Your Bussniss");

            _context.Favorites.Remove(item);
            _context.SaveChanges();
            return Ok("product Removed Successfully from your Favorites");
        }
    }
}
