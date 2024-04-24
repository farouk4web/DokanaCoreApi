using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Dokana.DTOs.Account
{
    public class ChangePasswordDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
