using System.ComponentModel.DataAnnotations;
using PCE_Web.Classes.ValidationAttributes;

namespace PCE_Web.Models
{
    public class EmailModel
    {
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Turite įrašyti email.")]
        [EmailSpelling]
        public string Email { get; set; }
    }

    public class CodeModel
    {
        [Display(Name = "Code")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Turite įvesti kodą, nusiųsta į jūsų email paštą.")]
        public string Code { get; set; }
    }

    public class PasswordModel
    {
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Turite įrašyti slaptažodį.")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Slaptažodis turi būti bet 4 simbolių ilgio.")]
        [PasswordSpelling]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Turite patvirtinti slaptažodį.")]
        [Compare("Password", ErrorMessage = "Slaptažodis turi sutapti su patvirtinimo slaptažodžiu.")]
        public string ConfirmPassword { get; set; }
    }
    public class InputModel
    {
        public EmailModel EmailModel { get; set; }
        public CodeModel CodeModel { get; set; }
        public PasswordModel PasswordModel { get; set; }

    }
}
