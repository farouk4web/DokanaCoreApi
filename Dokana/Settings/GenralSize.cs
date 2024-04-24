using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Dokana.Settings
{
    public class GenralSize : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not null)
            {
                var picture = value as IFormFile;
                long maximumPictureSize = 1048576;

                if (picture.Length > maximumPictureSize)
                    return new ValidationResult("your image should be less than 1 MB");
            }

            return ValidationResult.Success;
        }
    }
}