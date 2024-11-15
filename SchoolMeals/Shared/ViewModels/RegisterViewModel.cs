using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMeals.Shared.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Fältet \"Förnamn\" måste fyllas ...")]
        [Display(Name = "Namn")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Fältet \"Efternamn\" måste fyllas ...")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Fältet \"E-post\" måste fyllas ...")]
        [EmailAddress(ErrorMessage = "Skriv en korrekt e-post adress ... ")]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Fältet \"Lösenord\" måste fyllas ...")]
        [StringLength(100, ErrorMessage = "Lösenord måste vara minst 6 tecken långa.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Fältet \"Bekräfta lösenord\" måste fyllas ...")]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenordet och bekräftelseslösenordet matchar inte.")]
        public string ConfirmPassword { get; set; }

    }
}
