using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMeals.Shared.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Fältet \"E-post\" måste fyllas ...")]
        [EmailAddress(ErrorMessage = "Skriv en korrekt e-post adress ... ")]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Fältet \"Lösenord\" måste fyllas ...")]
        [StringLength(100, ErrorMessage = "Lösenord måste vara minst 6 tecken långa.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig?")]
        public bool Remember { get; set; }
    }
}
