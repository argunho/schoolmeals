using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMeals.Shared.Models
{
    public class Bookmarks
    {
        [Key]
        public int Id { get; set; }
        public School School { get; set; }
        public string UserId { get; set; }
    }
}
