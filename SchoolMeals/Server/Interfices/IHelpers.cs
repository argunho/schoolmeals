using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolMeals.Server.Interfices
{
    public interface IHelpers
    {
        dynamic SaveFile(byte[] file, string path);

        bool DeleteImage(string img);

        string ImgName(string name, string img);

    }
}
