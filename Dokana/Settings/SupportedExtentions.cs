using System.ComponentModel.DataAnnotations;

namespace Dokana.Settings
{
    public class SupportedExtentions : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                var picture = value as IFormFile;

                var pictureExtension = Path.GetExtension(picture.FileName.ToLower());
                var supportedTypes = new List<string> { ".png", ".jpg", ".jpeg" };

                if (!supportedTypes.Contains(pictureExtension))
                    return new ValidationResult("We support only '.png, .jpg, .jpeg' extentions only");
            }

            return ValidationResult.Success;
        }
    }
}
