using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using PCE_Web.Models;

namespace PCE_Web.Classes.ValidationAttributes
{
    public class EmailExistenceAttribute : ValidationAttribute
    {
        private readonly IDatabaseManager _databaseManager;

        public EmailExistenceAttribute()
        {
        }

        public EmailExistenceAttribute(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

            if (_databaseManager.CheckIfUserExists(email))
            {
                return new ValidationResult("Toks email jau panaudotas. Pabandykite kitą.");
            }
            else return ValidationResult.Success;
        }
    }
}
