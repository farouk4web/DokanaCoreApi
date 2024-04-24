using Dokana.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Dokana.DTOs.Account;
using Dokana.Services;
using Dokana.DTOs;

namespace Dokana.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppMethodsService _methods;
        private readonly IConfiguration _configuration;
        public AccountController(UserManager<ApplicationUser> userManager, IAppMethodsService methods, IConfiguration configuration)
        {
            _userManager = userManager;
            _methods = methods;
            _configuration = configuration;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // first check if we have any user with this email or username in Our Database
            if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                return BadRequest("email is already registered");

            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return BadRequest("Username is already Taken");

            //create new User And Push him to Database
            var newUser = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                JoinDate = DateTime.UtcNow,
                ProfileImageSrc = "/Uploads/Users/user.png"
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
            {
                var errors = new List<string>();

                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }

                return BadRequest(errors);
            }


            // create refresh Token and save it
            var newRefreshToken = CreateRefreshToken();
            newUser.RefreshToken = newRefreshToken.Token;
            newUser.RefreshTokenExpireDateTime = newRefreshToken.ExpireDateTime;
            await _userManager.UpdateAsync(newUser);

            // create json web token
            var clearToken = await CreateJwtToken(newUser);

            var responce = new TokenResponce
            {
                IsAuthenticated = true,

                Token = new JwtSecurityTokenHandler().WriteToken(clearToken),
                TokenExpiresOn = clearToken.ValidTo,

                RefreshToken = newUser.RefreshToken,
                RefreshTokenExpiresOn = newUser.RefreshTokenExpireDateTime,

                Email = newUser.Email,
                Username = newUser.UserName,
                Roles = new List<string>(),
                Messages = "Your Account Created Succefully"
            };

            return Ok(responce);
        }


        [HttpPost("Token")]
        public async Task<IActionResult> SignNewToken(LoginDto dto)
        {
            // first check if we have any user with this email or username in Our Database
            var userInDb = await _userManager.FindByEmailAsync(dto.Email);

            if (userInDb == null || !await _userManager.CheckPasswordAsync(userInDb, dto.Password))
                return BadRequest("email or password is not Correct");


            // create refresh Token and save it
            var newRefreshToken = CreateRefreshToken();
            userInDb.RefreshToken = newRefreshToken.Token;
            userInDb.RefreshTokenExpireDateTime = newRefreshToken.ExpireDateTime;
            await _userManager.UpdateAsync(userInDb);

            // create json web token
            var clearToken = await CreateJwtToken(userInDb);
            var userRoles = await _userManager.GetRolesAsync(userInDb);

            var responce = new TokenResponce
            {
                IsAuthenticated = true,

                Token = new JwtSecurityTokenHandler().WriteToken(clearToken),
                TokenExpiresOn = clearToken.ValidTo,

                RefreshToken = userInDb.RefreshToken,
                RefreshTokenExpiresOn = userInDb.RefreshTokenExpireDateTime,

                Email = userInDb.Email,
                Username = userInDb.UserName,
                Roles = userRoles
            };

            return Ok(responce);
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
        {
            if (dto.CurrentRefreshToken is null)
                return BadRequest("Invalid Refreesh Token");

            // first check if we have any user has this refresh Token in Our Database
            var userInDb = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == dto.CurrentRefreshToken);

            if (userInDb == null)
                return BadRequest("Invalid Refreesh Token");

            // now we find refresh token but its expired so we need to ===> revoke CurrentRefresh token
            if (userInDb.RefreshTokenExpireDateTime <= DateTime.UtcNow)
            {
                userInDb.RefreshToken = null;
                userInDb.RefreshTokenExpireDateTime = null;
                await _userManager.UpdateAsync(userInDb);

                return BadRequest("Invalid Refreesh Token");
            }

            // create refresh Token and save it
            var newRefreshToken = CreateRefreshToken();
            userInDb.RefreshToken = newRefreshToken.Token;
            userInDb.RefreshTokenExpireDateTime = newRefreshToken.ExpireDateTime;
            await _userManager.UpdateAsync(userInDb);

            // create access token
            var accessToken = await CreateJwtToken(userInDb);

            var userRoles = await _userManager.GetRolesAsync(userInDb);
            var responce = new TokenResponce
            {
                IsAuthenticated = true,

                TokenExpiresOn = accessToken.ValidTo,
                Token = new JwtSecurityTokenHandler().WriteToken(accessToken),

                RefreshToken = userInDb.RefreshToken,
                RefreshTokenExpiresOn = userInDb.RefreshTokenExpireDateTime,

                Email = userInDb.Email,
                Username = userInDb.UserName,
                Roles = userRoles
            };

            return Ok(responce);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");

            var currentUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == currentUserId);

            var result = await _userManager.ChangePasswordAsync(currentUser, dto.CurrentPassword, dto.NewPassword);

            if (result.Succeeded)
                return Ok("Your Password Changed Successfully.");

            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            var currentUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == currentUserId);

            var dto = new UserDto
            {
                Id = currentUserId,
                FullName = currentUser.FullName,
                Email = currentUser.Email,
                JoinDate = currentUser.JoinDate,
                ProfileImageSrc = currentUser.ProfileImageSrc
            };

            return Ok(dto);
        }

        [Authorize]
        [HttpPost("ChangeProfilePicture")]
        public async Task<IActionResult> ChangeProfilePicture([FromForm] ChangeProfilePicture dto)
        {
            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            var currentUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == currentUserId);

            currentUser.ProfileImageSrc = _methods.UploadPicture(dto.Picture, "Users", currentUserId);
            await _userManager.UpdateAsync(currentUser);

            return Ok(currentUser.ProfileImageSrc);
        }


        [Authorize]
        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var currentUserId = HttpContext.User.FindFirstValue("currentUserId");
            var currentUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == currentUserId);

            currentUser.FullName = dto.FullName;

            if (dto.Gender == 1)
                currentUser.Gender = "Male";
            else
                currentUser.Gender = "Female";

            await _userManager.UpdateAsync(currentUser);

            return Ok(new { FullName = currentUser.FullName, Gender = currentUser.Gender });
        }


        [Authorize]
        [HttpPost("LogOut")]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto dto)
        {
            // first check if we have any user with has this refresh Token in Our Database
            var userInDb = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == dto.CurrentRefreshToken);

            // need to make sure sender is the current user
            // EX: add in 'if' || userInDb.Id != currentUserId
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (userInDb == null || userInDb.Id != currentUser.Id)
                return BadRequest("Invalid Refreesh Token");

            // after this action success frontend should remove refresh token access token too 
            userInDb.RefreshToken = null;
            userInDb.RefreshTokenExpireDateTime = null;

            await _userManager.UpdateAsync(userInDb);

            return Ok("Your Refresh Token Revoked Successfully");
        }


        // internal Methods
        private NewRefreshTokenDto CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            var durationInHours = _configuration.GetValue<double>("JWT:RefreshTokenDurationInHours");

            var model = new NewRefreshTokenDto
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpireDateTime = DateTime.UtcNow.AddHours(durationInHours)
            };

            return model;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("currentUserId", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Key")));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("JWT:Issuer"),
                audience: _configuration.GetValue<string>("JWT:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>("JWT:DurationInMinutes")),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}