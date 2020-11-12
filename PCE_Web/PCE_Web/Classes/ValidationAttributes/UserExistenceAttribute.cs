using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using PCE_Web.Models;

namespace PCE_Web.Classes.ValidationAttributes
{
    public class UserExistenceAttribute : ValidationAttribute
    {
        private readonly IDatabaseManager _databaseManager;

        public UserExistenceAttribute()
        {
        }

        public UserExistenceAttribute(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

            if (!_databaseManager.CheckIfUserExists(email))
            {
                return new ValidationResult("Toks naudotojas neegzistuoja.");
            }
            else return ValidationResult.Success;
        }
    }
}
