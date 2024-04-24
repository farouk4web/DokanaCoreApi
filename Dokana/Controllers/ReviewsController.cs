using Dokana.DTOs;
using Dokana.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dokana.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // id of product not review
        [HttpGet("ProductReviews/{id}")]
        public IActionResult Getall4Product(int id)
        {
            var productInDb = _context.Products.Any(p => p.Id == id);
            if (!productInDb)
                return BadRequest("Product Id is not valid");

            var allReviewsinDb = _context.Reviews
                                                .Include(r => r.Buyer)
                                                .Where(r => r.ProductId == id)
                                                .ToList();

            var reviewsDto = allReviewsinDb.Select(r => new ReviewDto
            {
                Id = r.Id,
                Comment = r.Comment,
                DateOfCreate = r.DateOfCreate,
                StarsCount = r.StarsCount,
                ProductId = r.ProductId,
                BuyerId = r.BuyerId,

                BuyerDto = new UserDto
                {
                    Id = r.Buyer.Id,
                    FullName = r.Buyer.FullName,
                    Email = r.Buyer.Email,
                    JoinDate = r.Buyer.JoinDate,
                    ProfileImageSrc = r.Buyer.ProfileImageSrc
                }
            });

            return Ok(reviewsDto);
        }


        [HttpPost]
        [Authorize]
        public IActionResult AddNewReview(NewReviewDto dto)
        {
            // check if thir is a product with this id or NOT
            var productInDb = _context.Products
                                        .Include(p => p.Reviews)
                                        .SingleOrDefault(p => p.Id == dto.ProductId);

            if (productInDb is null)
                return BadRequest("thir is no product with this id");

            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");

            // check if user pay this product or not
            var userCartItems = _context.CartItems
                                            .Where(i => i.ProductId == dto.ProductId
                                                    && i.ShoppingCart.BuyerId == currentUserId
                                                    && i.ShoppingCart.IdOfOrder != null)
                                            .ToList();

            if (userCartItems.Count == 0)
                return BadRequest("You Should pay this product first");


            // check if user add review to this product before or not ------> just one review
            var isReviewBefore = _context.Reviews.Any(m => m.BuyerId == currentUserId && m.ProductId == dto.ProductId);

            if (isReviewBefore)
                return BadRequest("You cant add 2 review to one product");

            //add new review
            var newReview = new Review()
            {
                ProductId = dto.ProductId,
                Comment = dto.Comment,
                StarsCount = dto.StarsCount,

                DateOfCreate = DateTime.UtcNow,
                BuyerId = currentUserId
            };
            _context.Reviews.Add(newReview);
            _context.SaveChanges();

            // sign average of product stars Count 
            productInDb.StarsCount = productInDb.Reviews.Average(r => r.StarsCount);
            _context.SaveChanges();


            // populate data to send it Again To client
            var currentUser = _context.Users.Find(currentUserId);
            var reviewInDetails = new ReviewDto
            {
                Id = newReview.Id,
                DateOfCreate = newReview.DateOfCreate,
                BuyerId = newReview.BuyerId,
                ProductId = newReview.ProductId,
                StarsCount = newReview.StarsCount,
                Comment = newReview.Comment,
                BuyerDto = new UserDto
                {
                    Id = currentUser.Id,
                    Email = currentUser.Email,
                    FullName = currentUser.FullName,
                    ProfileImageSrc = currentUser.ProfileImageSrc,
                    JoinDate = currentUser.JoinDate
                }
            };

            return Ok(reviewInDetails);
        }
    }
}
