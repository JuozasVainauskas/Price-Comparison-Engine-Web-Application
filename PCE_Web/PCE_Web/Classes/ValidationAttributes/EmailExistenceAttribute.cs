using System.ComponentModel.DataAnnotations;
using PCE_Web.Models;

namespace PCE_Web.Classes.ValidationAttributes
{
    public class EmailExistenceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;
            var accountManager = (IAccountManager)validationContext.GetService(typeof(IAccountManager));

            if (accountManager != null && accountManager.CheckIfUserExists(email))
            {
                return new ValidationResult("Toks email jau panaudotas. Pabandykite kitą.");
            }
            else return ValidationResult.Success;
        }
    }
}
