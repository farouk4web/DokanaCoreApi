using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs
{
    public class UserRoleDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}