using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;

namespace SchoolMeals.Shared.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        [AllowHtml]
        public string Name { get; set; }
        [AllowHtml]
        public string Ingredients { get; set; }
        [AllowHtml]
        public string Text { get; set; }
        public string ImgUrl { get; set; }
        public School School { get; set; }
    }
}
