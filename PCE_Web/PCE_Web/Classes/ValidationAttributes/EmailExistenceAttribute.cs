using System.ComponentModel.DataAnnotations;
using PCE_Web.Models;

namespace PCE_Web.Classes.ValidationAttributes
{
    public class EmailExistenceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;
            var databaseManager = (IDatabaseManager)validationContext.GetService(typeof(IDatabaseManager));

            if (databaseManager != null && databaseManager.CheckIfUserExists(email))
            {
                return new ValidationResult("Toks email jau panaudotas. Pabandykite kitą.");
            }
            else return ValidationResult.Success;
        }
    }
}
