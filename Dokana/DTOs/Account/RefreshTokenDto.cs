using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs.Account
{
    public class RefreshTokenDto
    {
        [Required]
        public string CurrentRefreshToken { get; set; }
    }
}
