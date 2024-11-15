using SchoolMeals.Client.Helpers;
using SchoolMeals.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMeals.Client.Pages.Authantication
{
    public partial class PasswordReset
    {
        private FormParams Form { get; set; } = new FormParams();
        private PasswordResetViewModel Model { get; set; } = new PasswordResetViewModel();

        private void OnValidSubmit()
        {

        }
    }

    public class PasswordResetViewModel
    {
        [Required(ErrorMessage = "Fältet \"E-post\" måste fyllas ...")]
        [EmailAddress(ErrorMessage = "Skriv en korrekt e-post adress ... ")]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }
}
