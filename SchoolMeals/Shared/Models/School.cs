using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;

namespace SchoolMeals.Shared.Models
{
    public class School
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [AllowHtml]
        public string Name { get; set; }
        [AllowHtml]
        public string Text { get; set; }
        public string Place { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string Link { get; set; }
        public string ImgUrl { get; set; }
        public bool Open { get; set; }
        public Municipality Municipality { get; set; }
        public List<Users> Users { get; set; }
        public List<Meal> Meals { get; set; }
        public List<Bookmarks> Bookmarks { get; set; }
        public List<Recipe> Recipies { get; set; }
    }
}
