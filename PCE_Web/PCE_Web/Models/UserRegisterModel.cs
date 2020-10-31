using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PCE_Web.Models
{
    public class UserRegisterModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Privalote užpildyti email laukelį.")]
        public string Email { get; set; }
        [Display(Name = "Slaptažodis")]
        [Required(ErrorMessage = "Privalote užpildyti slaptažodžio laukelį.")]
        public string Password { get; set; }
        [Display(Name = "Slaptažodžio patvirtinimas")]
        [Required(ErrorMessage = "Privalote patvirtinti slaptažodį.")]
        public string ConfirmPassword { get; set; }
    }
}