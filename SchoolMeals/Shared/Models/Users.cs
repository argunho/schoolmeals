using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolMeals.Shared.Models
{
    public class Users : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime Date { get; set; }
        public School School { get; set; }
        public Users()
        {
            Date = DateTime.Now;
        }
    }
}
