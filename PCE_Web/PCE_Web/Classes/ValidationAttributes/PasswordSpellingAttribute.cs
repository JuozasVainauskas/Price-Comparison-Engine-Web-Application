using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PCE_Web.Classes.ValidationAttributes
{
    public class PasswordSpellingAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var pattern =
                new Regex(
                    @"(\.*\d+\.*[a-zA-Z]\.*[a-zA-Z]\.*[a-zA-Z]\.*)|(\.*[a-zA-Z]\.*\d+\.*[a-zA-Z]\.*[a-zA-Z]\.*)|(\.*[a-zA-Z]\.*[a-zA-Z]\.*\d+\.*[a-zA-Z]\.*)|(\.*[a-zA-Z]\.*[a-zA-Z]\.*[a-zA-Z]\.*\d+\.*)",
                    RegexOptions.Compiled);

            var password = value as string;

            if (string.IsNullOrWhiteSpace(password))
            {
                return new ValidationResult("Turite įrašyti slaptažodį.");
            }
            else if (!pattern.IsMatch(password))
            {
                return new ValidationResult("Slaptažodyje turi būti bent trys raidės ir vienas skaičius.");
            }
            else return ValidationResult.Success;
        }
    }
}
