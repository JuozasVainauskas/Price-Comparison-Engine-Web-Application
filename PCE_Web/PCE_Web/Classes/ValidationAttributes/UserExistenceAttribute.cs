using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using PCE_Web.Models;

namespace PCE_Web.Classes.ValidationAttributes
{
    public class UserExistenceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;
            var accountManager = (IAccountManager)validationContext.GetService(typeof(IAccountManager));

            if (accountManager != null && !accountManager.CheckIfUserExists(email))
            {
                return new ValidationResult("Toks naudotojas neegzistuoja.");
            }
            else return ValidationResult.Success;
        }
    }
}
