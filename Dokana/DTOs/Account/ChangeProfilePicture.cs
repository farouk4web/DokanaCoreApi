using Dokana.Settings;
using System.ComponentModel.DataAnnotations;

namespace Dokana.DTOs.Account
{
    public class ChangeProfilePicture
    {
        [GenralSize]
        [SupportedExtentions]
        [Required(ErrorMessage = "You should Choose Your new Picture First (:")]
        public IFormFile Picture { get; set; }
    }
}
