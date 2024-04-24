using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dokana.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireDateTime { get; set; }


        [Required, StringLength(60, MinimumLength = 4), RegularExpression("^[a-zA-Zء-ي ]*$")]
        public string FullName { get; set; }

        public string Gender { get; set; }

        public string ProfileImageSrc { get; set; }

        public DateTime JoinDate { get; set; }
    }
}
