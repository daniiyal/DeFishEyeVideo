using ASPdeFish.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASPdeFish.Controllers
{
    public class UploadController : HomeController
    {
        public UploadController(IWebHostEnvironment env) : base(env)
        {
            _env = env;
        }

        public async Task<IActionResult> UploadVideo(IFormFile video)
        {
            if (video == null)
            {
                return View("../Home/Index");
            }
            var name = video.FileName.Split('.').First();

            var videoPath = Path.Combine(_env.WebRootPath, "videos", video.FileName);
            using (FileStream fileStream = new(videoPath, FileMode.Create))
            {
                await video.CopyToAsync(fileStream);
            }

            string ffmpegPath = Path.Combine(_env.ContentRootPath, "ffmpeg", "ffmpeg.exe");

            string thumbName = $"{name}thumb.png";

            string thumbPath = Path.Combine(_env.WebRootPath, "images", thumbName);

            string copyPath = Path.Combine(_env.WebRootPath, @"images\thumbcopy.png");

            GetThumb(videoPath, thumbPath, ffmpegPath);

            videoModel = new(thumbName, video.FileName);

            FileInfo fileInf = new FileInfo(thumbPath);
            fileInf.CopyTo(copyPath, true);

            HttpContext.Session.SetString("thumbName", thumbName);
            HttpContext.Session.SetString("videoName", video.FileName);
            //DeFishEyeAlgorithm.deFishEyeImage(thumbPath);

            return View("../Home/Index", videoModel);
        }
        public void GetThumb(string videoPath, string thumbPath, string ffmpegPath)
        {
            string arg = @$"-y -i {videoPath} -vframes 1 {thumbPath}";
            Console.WriteLine(arg);
            var ffmpeg = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arg,
                WorkingDirectory = _env.WebRootPath,
                CreateNoWindow = true,
                UseShellExecute = true
            };

            using (var process = new Process { StartInfo = ffmpeg })
            {
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
