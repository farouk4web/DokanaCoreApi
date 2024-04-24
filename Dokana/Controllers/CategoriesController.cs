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
    [Authorize(Roles = RoleName.OwnersAndAdminsAndSellers)]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(int pageNumber = 1)
        {
            int elementInRequist = 25;
            var groupOfCategories = _context.Categories
                                                .OrderBy(p => p.Id)
                                                .Skip((pageNumber - 1) * elementInRequist)
                                                .Take(elementInRequist)
                                                .ToList();

            var categoriesDto = groupOfCategories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });

            return Ok(categoriesDto);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var categoryInDb = _context.Categories.SingleOrDefault(p => p.Id == id);
            if (categoryInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");

            var categoryDto = new CategoryDto
            {
                Id = categoryInDb.Id,
                Name = categoryInDb.Name,
                Description = categoryInDb.Description
            };

            return Ok(categoryDto);
        }

        [HttpPost]
        public IActionResult New(CategoryDto categoryDto)
        {
            Category newCategory = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            _context.Categories.Add(newCategory);
            _context.SaveChanges();

            categoryDto.Id = newCategory.Id;

            return Ok(categoryDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CategoryDto dto)
        {
            var categoryInDb = _context.Categories.SingleOrDefault(p => p.Id == id);
            if (categoryInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");

            // UPDATE new data
            categoryInDb.Name = dto.Name;
            categoryInDb.Description = dto.Description;

            _context.SaveChanges();

            dto.Id = categoryInDb.Id;  // maybe you need to remove this line

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var categoryInDb = _context.Categories.SingleOrDefault(p => p.Id == id);

            if (categoryInDb is null)
                return NotFound("sorry we dont found what you are looking for ):");

            var categoryProductsCount = _context.Products.Where(p => p.CategoryId == categoryInDb.Id).Count();
            if (categoryProductsCount > 0)
                return BadRequest("You cant remove any category had 1 product or more.");

            _context.Categories.Remove(categoryInDb);
            _context.SaveChanges();

            return Ok("Category Removed Successfully");
        }
    }
}
