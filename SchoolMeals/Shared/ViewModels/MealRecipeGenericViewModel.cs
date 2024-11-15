using Microsoft.AspNetCore.Http;
using SchoolMeals.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SchoolMeals.Shared.ViewModels
{
    public class MealRecipeGenericViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] File { get; set; }
        public string ImgUrl { get; set; }
        public School School { get; set; }
        public int RecipeId { get; set; }
        public string DayOfWeek { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Fält \"Namn\" måste fyllas ...")]
        public string Name { get; set; }

        [AllowHtml]
        public string Ingredients { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Fält \"Text\" måste fyllas ...")]
        public string Text { get; set; }

        [Required(ErrorMessage = "En skola från listan måste bli selected ...")]
        public int SchoolId { get; set; }

        [Required(ErrorMessage = "Datum måste väljas ...")]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}