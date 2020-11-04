using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PCE_Web.Classes.ValidationAttributes
{
    public class EmailExistenceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

            if (DatabaseManager.CheckIfUserExists(email))
            {
                return new ValidationResult("Toks email jau panaudotas. Pabandykite kitą.");
            }
            else return ValidationResult.Success;
        }
    }
}
