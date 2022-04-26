using ASPdeFish.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASPdeFish.Controllers
{
    public class VideoController : HomeController
    {
        public VideoController(IWebHostEnvironment env) : base(env)
        {
            _env = env;
        }

        [HttpPost]
        public IActionResult ChangeVideo(string factor)
        {

            double f = Convert.ToDouble(factor.Replace(".", ","));
            if (HttpContext.Session != null)
            {
                videoModel = new Video(HttpContext.Session.GetString("thumbName"),
                                        HttpContext.Session.GetString("videoName"));
            }

            if (videoModel.ThumbName == null)
            {
                return View("../Home/Index");
            }
            var videoName = videoModel.VideoName.Split('.').First();
            var mime = videoModel.VideoName.Split('.').Last();

            string videoPath = Path.Combine(_env.WebRootPath, "videos", videoModel.VideoName);
            string framePath = Path.Combine(_env.WebRootPath, "frames");

            string audioPath = Path.Combine(_env.WebRootPath, "audio", $"{videoName}.aac");

            DeFishEyeAlgorithm.SplitVideo(videoPath, framePath);
            DeFishEyeAlgorithm.ExtractAudio(videoPath, audioPath);

            string[] images = Directory.GetFiles(Path.Combine(_env.WebRootPath, "frames"));

            Parallel.ForEach(images, i => {
                DeFishEyeAlgorithm.deFishEye(i, f);
            });

            string outVideoName = $"{videoName}_new.{mime}";
            string outVideoWOAudioPath = Path.Combine(_env.WebRootPath, "videos", $"{videoName}_wo_audio.{mime}");
            string outVideoPath = Path.Combine(_env.WebRootPath, "videos", outVideoName);

            DeFishEyeAlgorithm.MergeFrames(framePath, outVideoWOAudioPath);
            DeFishEyeAlgorithm.MergeAudio(outVideoWOAudioPath, audioPath, outVideoPath);

            videoModel.VideoName = outVideoName;

            DirectoryInfo dir = new DirectoryInfo(framePath);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

            return View("../Home/Index", videoModel);
        }

    }
}
