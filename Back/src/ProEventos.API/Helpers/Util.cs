using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ProEventos.API.Helpers
{
    public class Util : IUtil
    {
        private readonly IWebHostEnvironment hostEnvironment;
        public Util(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;            
        }

        public async Task<string> SaveImage(IFormFile imageFile, string destino, string newFileName = "")
        {
            
            //imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');

            var imageName = String.IsNullOrEmpty(newFileName) ? new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-') : newFileName;

            imageName = $"{imageName}{DateTime.UtcNow.ToString("yyyymmddssfff")}{Path.GetExtension(imageFile.FileName)}";

            var imagePath = Path.Combine(hostEnvironment.ContentRootPath, @$"Resources/{destino}", imageName);

            using( var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;
        }

        public void DeleteImage(string imageFile, string destino)
        {
            var imagePath = Path.Combine(hostEnvironment.ContentRootPath, @$"Resources/{destino}", imageFile);
            if (System.IO.File.Exists(imagePath)) 
                System.IO.File.Delete(imagePath);
        }
        
    }
}