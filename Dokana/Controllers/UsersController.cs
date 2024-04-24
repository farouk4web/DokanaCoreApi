using Dokana.DTOs;
using Dokana.Models;
using Dokana.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dokana.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Authorize(Roles = RoleName.OwnersAndAdminsAndSellersAndShippingStaff)]
        public async Task<IActionResult> Index(string filter = "all", int pageNumber = 1)
        {
            int usersPerRequist = 10;
            IList<ApplicationUser> groupOfUsers;

            switch (filter)
            {
                case "owners":
                    groupOfUsers = await _userManager.GetUsersInRoleAsync(RoleName.Owners);
                    break;

                case "admins":
                    groupOfUsers = await _userManager.GetUsersInRoleAsync(RoleName.Admins);
                    break;

                case "sellers":
                    groupOfUsers = await _userManager.GetUsersInRoleAsync(RoleName.Sellers);
                    break;

                case "shippingStaff":
                    groupOfUsers = await _userManager.GetUsersInRoleAsync(RoleName.ShippingStaff);
                    break;

                default:
                    groupOfUsers = _context.Users.ToList();
                    break;
            }

            // create pagenation
            groupOfUsers = groupOfUsers
                                     .OrderBy(u => u.JoinDate)
                                     .Skip((pageNumber - 1) * usersPerRequist)
                                     .Take(usersPerRequist)
                                     .ToList();

            var usersDto = groupOfUsers.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                JoinDate = u.JoinDate,
                ProfileImageSrc = u.ProfileImageSrc
            });

            return Ok(usersDto);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> UserAccount(string id)
        {
            var userInDb = _context.Users.SingleOrDefault(u => u.Id == id);

            if (userInDb is null)
                return NotFound("Sorry, We dont find any user with this id ):");

            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            var currentUser = _context.Users.Find(currentUserId);

            IList<string> roles = new List<string>();
            if (User.IsInRole(RoleName.Owners) || User.IsInRole(RoleName.Admins) || User.IsInRole(RoleName.Sellers) || User.IsInRole(RoleName.ShippingStaff))
                roles = await _userManager.GetRolesAsync(userInDb);

            var userDto = new
            {
                Id = userInDb.Id,
                FullName = userInDb.FullName,
                Email = userInDb.Email,
                JoinDate = userInDb.JoinDate,
                ProfileImageSrc = userInDb.ProfileImageSrc,
                Roles = roles
            };

            return Ok(userDto);
        }


        [HttpPost("AddToRole")]
        [Authorize(Roles = RoleName.OwnersAndAdmins)]
        public async Task<IActionResult> AddUserToRole(UserRoleDto dto)
        {
            // check if user and role are exist or not
            var userInDb = _context.Users.Find(dto.UserId);
            if (userInDb is null || !await _roleManager.RoleExistsAsync(dto.RoleName))
                return BadRequest("Invalid User id or role Name");

            if (await _userManager.IsInRoleAsync(userInDb, dto.RoleName))
                return Ok($"{userInDb.FullName} is already assigned to this role");

            if (dto.RoleName == RoleName.Owners && !User.IsInRole(RoleName.Owners))
                return BadRequest("You are just admin, You Dont have Primtion to Do this):");


            var result = await _userManager.AddToRoleAsync(userInDb, dto.RoleName);

            if (result.Succeeded)
                return Ok($"{userInDb.FullName} added successfully to {dto.RoleName}");
            else
                return BadRequest("Something went wrong");
        }


        [HttpPost("RemoveFromRole")]
        [Authorize(Roles = RoleName.OwnersAndAdmins)]
        public async Task<IActionResult> RemoveUserFromRole(UserRoleDto dto)
        {
            // check if user and role are exist or not
            var userInDb = _context.Users.Find(dto.UserId);
            if (userInDb is null || !await _roleManager.RoleExistsAsync(dto.RoleName))
                return BadRequest("Invalid User id or role Name");

            if (dto.RoleName == RoleName.Owners && !User.IsInRole(RoleName.Owners))
                return BadRequest("You are just admin, You Dont have Primtion to Do this):");


            if (await _userManager.IsInRoleAsync(userInDb, dto.RoleName))
            {
                var result = await _userManager.RemoveFromRoleAsync(userInDb, dto.RoleName);

                if (result.Succeeded)
                    return Ok($"{userInDb.FullName} removed successfully from {dto.RoleName}");
                else
                    return BadRequest("Something went wrong");
            }

            return BadRequest("Something went wrong");
        }



        // in need this action to create Main Roles and Owner User
        //[HttpPost("InDev")]
        //public async Task<IActionResult> CreateRoles()
        //{
        //    List<string> roles = new List<string>
        //    {
        //        RoleName.Owners,
        //        RoleName.Admins,
        //        RoleName.Sellers,
        //        RoleName.ShippingStaff
        //    };

        //    // Create Roles
        //    foreach (var role in roles)
        //    {
        //        IdentityRole newRole = new IdentityRole
        //        {
        //            Name = role
        //        };

        //        await _roleManager.CreateAsync(newRole);
        //    }

        //    // Add Owner To Owners Role
        //    var user = await _userManager.FindByNameAsync("farouk");
        //    await _userManager.AddToRoleAsync(user, RoleName.Owners);

        //    return Ok("We Do It");
        //}


    }
}