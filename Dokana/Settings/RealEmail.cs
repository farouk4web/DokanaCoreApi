using System.ComponentModel.DataAnnotations;

namespace Dokana.Settings
{
    public class RealEmail : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

            if (email.Contains("@gmail.com") ||email.Contains("@yahoo.com") ||email.Contains("@hotmail.com") ||email.Contains("@outlook.com"))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("we support {Gmail, hotmail, yahoo, outlook} Only (:");
        }
    }
}
