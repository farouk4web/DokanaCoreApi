using Dokana.DTOs;
using Dokana.DTOs.Product;
using Dokana.Models;
using Dokana.Services;
using Dokana.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dokana.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize(Roles = RoleName.OwnersAndAdminsAndSellers)]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppMethodsService _methods;
        public ProductsController(ApplicationDbContext context, IAppMethodsService methods)
        {
            _context = context;
            _methods = methods;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(int categoryId = 0, int pageNumber = 1)
        {
            var groupOfProducts = new List<Product>();
            int elementInPage = 10;

            if (categoryId == 0)
            {
                groupOfProducts = _context.Products
                                                    .Include(p => p.Category)
                                                    .OrderBy(p => p.Id)
                                                    .Skip((pageNumber - 1) * elementInPage)
                                                    .Take(elementInPage)
                                                    .ToList();
            }
            else
            {
                groupOfProducts = _context.Products
                                                    .Include(p => p.Category)
                                                    .Where(p => p.CategoryId == categoryId)
                                                    .OrderBy(p => p.Id)
                                                    .Skip((pageNumber - 1) * elementInPage)
                                                    .Take(elementInPage)
                                                    .ToList();
            }

            var productsDto = groupOfProducts.Select(p => new ProductDetailsDto
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
                SellerId = p.SellerId,
                CategoryId = p.CategoryId,

                CategoryDto = new CategoryDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name
                }
            });

            return Ok(productsDto);
        }


        [HttpGet("OnSlider")]
        [AllowAnonymous]
        public IActionResult OnSlider()
        {
            var productsOnSlider = _context.Products
                                                  .Where(s => s.ShowInSlider)
                                                  .OrderBy(p => p.Id)
                                                  .ToList();


            var productsDto = productsOnSlider.Select(p => new ProductDetailsDto
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
                SellerId = p.SellerId,
                CategoryId = p.CategoryId
            });

            return Ok(productsDto);
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var productInDb = _context.Products
                                            .Include(p => p.Category)
                                            .SingleOrDefault(p => p.Id == id);
            if (productInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");

            var productDto = new ProductDetailsDto
            {
                Id = productInDb.Id,
                Name = productInDb.Name,
                ImageSrc = productInDb.ImageSrc,
                Price = productInDb.Price,
                Description = productInDb.Description,
                AvailableToSale = productInDb.AvailableToSale,
                CountOfSale = productInDb.CountOfSale,
                UnitsInStore = productInDb.UnitsInStore,
                ShowInSlider = productInDb.ShowInSlider,
                StarsCount = productInDb.StarsCount,
                SellerId = productInDb.SellerId,
                CategoryId = productInDb.CategoryId,

                CategoryDto = new CategoryDto
                {
                    Id = productInDb.Id,
                    Name = productInDb.Category.Name
                }

            };

            return Ok(productDto);
        }


        [HttpPost]
        public IActionResult New([FromForm] NewProductDto newProductDto)
        {
            if (!_context.Categories.Any(c => c.Id == newProductDto.CategoryId))
                return BadRequest("Category Id Is Not Valid");

            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            var newProduct = new Product
            {
                Name = newProductDto.Name,
                Description = newProductDto.Description,
                AvailableToSale = newProductDto.AvailableToSale,
                CategoryId = newProductDto.CategoryId,
                Price = newProductDto.Price,
                ShowInSlider = newProductDto.ShowInSlider,
                UnitsInStore = newProductDto.UnitsInStore,
                ImageSrc = "/Uploads/Products/product.png",

                SellerId = currentUserId
            };
            _context.Products.Add(newProduct);
            _context.SaveChanges();


            // upload product image
            newProduct.ImageSrc = _methods.UploadPicture(newProductDto.Picture, "Products", newProduct.Id.ToString());
            _context.SaveChanges();

            var category = _context.Categories.Find(newProduct.CategoryId);

            // populate Compelete data in DTO
            var dto = new ProductDetailsDto
            {
                Id = newProduct.Id,
                Name = newProduct.Name,
                Description = newProduct.Description,
                AvailableToSale = newProduct.AvailableToSale,
                Price = newProduct.Price,
                UnitsInStore = newProduct.UnitsInStore,
                ShowInSlider = newProduct.ShowInSlider,
                StarsCount = newProduct.StarsCount,
                CountOfSale = newProduct.CountOfSale,
                ImageSrc = newProduct.ImageSrc,
                SellerId = newProduct.SellerId,
                CategoryId = newProduct.CategoryId,

                CategoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                }
            };

            return Ok(dto);
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] UpdateProductDto updateProductDto)
        {
            var productInDb = _context.Products
                                        .Include(p => p.Category)
                                        .SingleOrDefault(p => p.Id == id);
            if (productInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");

            if (!_context.Categories.Any(c => c.Id == updateProductDto.CategoryId))
                return BadRequest("Category Id Is Not Valid");

            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");

            if (productInDb.SellerId == currentUserId || User.IsInRole(RoleName.Admins) || User.IsInRole(RoleName.Owners))
            {
                if (updateProductDto.Picture is not null)
                    productInDb.ImageSrc = _methods.UploadPicture(updateProductDto.Picture, "Products", productInDb.Id.ToString());


                productInDb.Name = updateProductDto.Name;
                productInDb.Description = updateProductDto.Description;
                productInDb.Price = updateProductDto.Price;
                productInDb.UnitsInStore = updateProductDto.UnitsInStore;
                productInDb.AvailableToSale = updateProductDto.AvailableToSale;
                productInDb.ShowInSlider = updateProductDto.ShowInSlider;
                productInDb.CategoryId = updateProductDto.CategoryId;

                _context.SaveChanges();


                // populate Compelete data in DTO
                var category = _context.Categories.Find(productInDb.CategoryId);
                var dto = new ProductDetailsDto
                {
                    Id = productInDb.Id,
                    Name = productInDb.Name,
                    Description = productInDb.Description,
                    AvailableToSale = productInDb.AvailableToSale,
                    Price = productInDb.Price,
                    UnitsInStore = productInDb.UnitsInStore,
                    ShowInSlider = productInDb.ShowInSlider,
                    StarsCount = productInDb.StarsCount,
                    CountOfSale = productInDb.CountOfSale,
                    ImageSrc = productInDb.ImageSrc,
                    SellerId = productInDb.SellerId,
                    CategoryId = productInDb.CategoryId,

                    CategoryDto = new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                    }
                };

                return Ok(dto);
            }

            return BadRequest("something went wrong ):");
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // I think we dont need to use it we can just edit the product and make is as NotAvailable To Bay 
            var productInDb = _context.Products
                                        .Include(p => p.CartItems)
                                        .ThenInclude(cart => cart.ShoppingCart)
                                        .SingleOrDefault(p => p.Id == id);
            if (productInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");


            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            if (productInDb.SellerId == currentUserId || User.IsInRole(RoleName.Admins) || User.IsInRole(RoleName.Owners))
            {
                if (productInDb.CartItems.Count > 0)
                {
                    foreach (var item in productInDb.CartItems)
                    {
                        if (item.ShoppingCart.IdOfOrder is not null)
                        {
                            var orderState = _context.Orders.Find(item.ShoppingCart.IdOfOrder).IsConfirmed;
                            if (orderState)
                                return Ok("You cant remove this product at now because it was related with an order, and it was confirmed");
                        }
                    }
                }

                // remove Image from server file
                _methods.RemovePicture(productInDb.ImageSrc);

                // not related with any order
                _context.Products.Remove(productInDb);
                _context.SaveChanges();

                return Ok("product Removed Successfully");
            }

            return BadRequest("something went wrong");
        }
    }
}