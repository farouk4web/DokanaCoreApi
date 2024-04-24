using Dokana.Settings;
using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs.Account
{
    public class RegisterDto
    {
        [Required, StringLength(60, MinimumLength = 4)]
        [RegularExpression("^[a-zA-ZÁ-í ]*$")]
        public string FullName { get; set; }


        [Required, StringLength(50, MinimumLength = 2)]
        public string Username { get; set; }


        [Required, EmailAddress, RealEmail]
        public string Email { get; set; }


        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}