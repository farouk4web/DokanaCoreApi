using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }

        [Required, StringLength(60, MinimumLength = 4), RegularExpression("^[a-zA-Zء-ي ]*$")]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string ProfileImageSrc { get; set; }

        public DateTime? JoinDate { get; set; }
    }
}