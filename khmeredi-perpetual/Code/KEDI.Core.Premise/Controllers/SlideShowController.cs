using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Files;
using KEDI.Core.Premise.Models.SlideShow;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FileIo = System.IO.File;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class SlideShowController : Controller
    {
        private readonly ILogger<SlideShowController> _logger;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly PosRetailModule _posRetail;
        public SlideShowController(ILogger<SlideShowController> logger, PosRetailModule posRetail, IWebHostEnvironment hostEnv)
        {
            _logger = logger;
            _posRetail = posRetail;
            _hostEnv = hostEnv;
        }

        private string GetImagePath()
        {
            var path = Path.Combine(_hostEnv.WebRootPath, "Images/slides");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public IActionResult UploadImages()
        {
            ViewBag.SlideShow = "highlight";
            string[] imgNames = _posRetail.GetSlideImageNames().Select(n => Path.GetFileName(n)).ToArray();
            var fileCollection = new FileCollection<SlideImage>
            {
                Items = imgNames.Select(name =>
                {
                    long filesizeBytes = new FileInfo($"{GetImagePath()}/{name}").Length;
                    long fileSizeKB = filesizeBytes / 1024;
                    var slideImg = new SlideImage
                    {
                        Name = name,
                        Size = $"{fileSizeKB} KB ({filesizeBytes} bytes)",
                        Selected = true
                    };
                    return slideImg;
                }).ToArray()
            };
            return View(fileCollection);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImages(FileCollection<SlideImage> slideImages)
        {
            if (slideImages.Files == null || slideImages.Items == null)
            {
                return RedirectToAction(nameof(UploadImages));
            }
            if (slideImages.Files.Count <= 0) { return RedirectToAction(nameof(UploadImages)); }
            string path = GetImagePath();
            for (int i = 0; i < slideImages.Files.Count; i++)
            {
                IFormFile file = slideImages.Files[i];
                string fileName = $"{path}/{Regex.Replace($"{file.FileName}", "\\s+", string.Empty)}";
                if (FileIo.Exists(fileName))
                {
                    SlideImage deletedImg = slideImages.Items.FirstOrDefault(s => s.Name == file.FileName && !s.Selected);
                    if (deletedImg != null)
                    {
                        FileIo.Delete(fileName);
                        continue;
                    }

                    // string ext = Path.GetExtension(fileName)?? string.Empty;
                    // string name = Path.GetFileNameWithoutExtension(fileName)?? string.Empty;
                    // fileName = $"{path}/{name}-Copy({DateTime.Today.ToString("yyyy-MM-dd")}){ext}";
                }

                using (Stream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fs);
                }
            }
            return RedirectToAction(nameof(UploadImages));
        }

        public IActionResult GetImageNames()
        {
            var imageNames = _posRetail.GetSlideImageNames();
            return Ok(imageNames);
        }
    }
}