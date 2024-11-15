using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SchoolMeals.Server.Data;
using SchoolMeals.Server.Interfices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMeals.Server.Repositiory
{
    public class HelpRepository : IHelpers
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _host;

        public HelpRepository(
                        AppDbContext db,
                        IWebHostEnvironment host
            )
        {
            _db = db;
            _host = host;
        }

        #region Handle Image
        // Return image path name
        public string ImgName(string name, string img)
        {
            
            var imgPath = "/images/" + name.ToLower().Replace(" ", "_").Replace("ä","a").Replace("ö","o").Replace("å","a") + "_" + 
                                        (DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss")).Replace(".","").Replace(" ","") + 
                                        img.Substring(img.LastIndexOf("."));
            return imgPath.ToLower();
        }

        // Save Uploaded File
        public dynamic SaveFile(byte[] file, string imgName)
        {
            try
            {
                var path = @"wwwroot" + imgName;
                //var path = $"{_host.WebRootPath}\\{imgName.ToLower().Replace("/,\\")}";
                var fs = System.IO.File.Create(path);
                fs.Write(file, 0, file.Length);
                fs.Close();
                return true;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // Delete image
        public bool DeleteImage(string img)
        {
            try
            {
                //var image = Path.Combine(_host.WebRootPath, img);
                var image = @"wwwroot" + img;
                FileInfo fi = new FileInfo(image);
                if (fi != null)
                {
                    System.IO.File.Delete(image);
                    fi.Delete();
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
