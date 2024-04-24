using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs
{
    public class UpdateProfileDto
    {
        [Required, StringLength(60, MinimumLength = 4), RegularExpression("^[a-zA-Zء-ي ]*$")]
        public string FullName { get; set; }


        [Range(1, 2,ErrorMessage = "Gender should be 1 for 'Male' and 2 for 'Female'")]
        public int Gender { get; set; }
    }
}
