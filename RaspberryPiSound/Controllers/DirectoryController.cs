using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using RaspberryPiSound.Properties;
using RaspberryPiSound.Models;

namespace RaspberryPiSound.Controllers
{
    public class DirectoryController : ApiController
    {
        [HttpGet]
        public dynamic Get([FromUri] string path)
        {
            var currentPath = Settings.Default.BasePath;
            if (!string.IsNullOrEmpty(path))
                currentPath = Path.Combine(currentPath, path);

            return new
            {
                Directories = Directory.GetDirectories(currentPath).Select(x => new PathDescription
                {
                    FullPath = x,
                    Name = new DirectoryInfo(x).Name
                }),
                Files = Directory.GetFiles(currentPath, "*.mp3").Select(x => new PathDescription
                {
                    FullPath = x,
                    Name = Path.GetFileName(x)
                }),
                CurrentPath = path ?? string.Empty
            };
        }
    }
}
