using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using SchoolMeals.Shared.Models;

namespace SchoolMeals.Shared.ViewModels
{
    public class SchoolViewModel
    {
        public int Id { get; set; }
      
        [AllowHtml] 
        [Required(ErrorMessage = "Skolans namn krävs ...")]
        public string Name { get; set; }
        [AllowHtml]
        public string Text { get; set; }
        public string Link { get; set; }
        public string FileName { get; set; }
        public byte[] File { get; set; }
        public string Place { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string ImgUrl { get; set; }
        public bool Open { get; set; }
        [Required(ErrorMessage = "Välj en kommun från listan ...")]
        public int MunicipalityId { get; set; }
        public Municipality Municipality { get; set; }
        public string UserEmail { get; set; }
    }
}
