using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PCE_Web.Classes.ValidationAttributes
{
    public class EmailSpellingAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var pattern = new Regex(@"([a-zA-Z0-9._-]*[a-zA-Z0-9][a-zA-Z0-9._-]*)(@gmail.com)$", RegexOptions.Compiled);

            var email = value as string;

            if (string.IsNullOrWhiteSpace(email))
            {
                return new ValidationResult("Turite įrašyti email.");
            }
            else if (!pattern.IsMatch(email))
            {
                return new ValidationResult("Email turi būti rašomas tokia tvarka:\nTuri sutapti su jūsų naudojamu gmail,\nkitaip negalėsite patvirtinti registracijos,\nTuri būti naudojamos raidės arba skaičiai,\nTuri būti nors vienas skaičius arba raidė,\nEmail'o pabaiga turi baigtis: @gmail.com, pvz.: kazkas@gmail.com.");
            }
            else return ValidationResult.Success;
        }
    }
}
