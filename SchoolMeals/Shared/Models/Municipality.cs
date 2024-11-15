using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMeals.Shared.Models
{
    public class Municipality
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<School> Schools { get; set; }
    }
}
