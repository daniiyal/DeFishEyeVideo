using ASPdeFish.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;




namespace ASPdeFish.Controllers
{

    public class HomeController : Controller
    {
        protected IWebHostEnvironment _env;
        protected Video videoModel;

        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Meme()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult ChangeVideo(string factor)
        //{

        //    double f = Convert.ToDouble(factor.Replace(".", ","));
        //    if (HttpContext.Session != null)
        //    {
        //        videoModel = new Video(HttpContext.Session.GetString("thumbName"),
        //                                HttpContext.Session.GetString("videoName"));
        //    }

        //    if (videoModel.ThumbName == null)
        //    {
        //        return View("Index");
        //    }
        //    var videoName = videoModel.VideoName.Split('.').First();
        //    var mime = videoModel.VideoName.Split('.').Last();

        //    string videoPath = Path.Combine(_env.WebRootPath, "videos", videoModel.VideoName);
        //    string framePath = Path.Combine(_env.WebRootPath, "frames");

        //    string audioPath = Path.Combine(_env.WebRootPath, "audio", $"{videoName}.aac");

        //    DeFishEyeAlgorithm.SplitVideo(videoPath, framePath);
        //    DeFishEyeAlgorithm.ExtractAudio(videoPath, audioPath);

        //    string[] images = Directory.GetFiles(Path.Combine(_env.WebRootPath, "frames"));

        //    Parallel.ForEach(images, i => {
        //        DeFishEyeAlgorithm.deFishEye(i, f);
        //    });

        //    string outVideoName = $"{videoName}_new.{mime}";
        //    string outVideoWOAudioPath = Path.Combine(_env.WebRootPath, "videos", $"{videoName}_wo_audio.{mime}");
        //    string outVideoPath = Path.Combine(_env.WebRootPath, "videos", outVideoName);

        //    DeFishEyeAlgorithm.MergeFrames(framePath, outVideoWOAudioPath);
        //    DeFishEyeAlgorithm.MergeAudio(outVideoWOAudioPath, audioPath, outVideoPath);

        //    videoModel.VideoName = outVideoName;
       
        //    return View("Index", videoModel);
        //}

        //public async Task<IActionResult> UploadVideo(IFormFile video)
        //{
        //    if (video == null)
        //    {
        //        return View("Index");
        //    }
        //    var name = video.FileName.Split('.').First();

        //    var videoPath = Path.Combine(_env.WebRootPath, "videos", video.FileName);
        //    using (FileStream fileStream = new(videoPath, FileMode.Create))
        //    {
        //        await video.CopyToAsync(fileStream);
        //    }

        //    string ffmpegPath = Path.Combine(_env.ContentRootPath, "ffmpeg", "ffmpeg.exe");
            
        //    string thumbName = $"{name}thumb.png";

        //    string thumbPath = Path.Combine(_env.WebRootPath, "images", thumbName);

        //    string copyPath = Path.Combine(_env.WebRootPath, @"images\thumbcopy.png");

        //    GetThumb(videoPath, thumbPath, ffmpegPath);

        //    videoModel = new(thumbName, video.FileName);

        //    FileInfo fileInf = new FileInfo(thumbPath);
        //    fileInf.CopyTo(copyPath, true);

        //    HttpContext.Session.SetString("thumbName", thumbName);
        //    HttpContext.Session.SetString("videoName", video.FileName);
        //    //DeFishEyeAlgorithm.deFishEyeImage(thumbPath);
            
        //    return View("Index", videoModel);
        //}
        //public void GetThumb(string videoPath, string thumbPath, string ffmpegPath)
        //{
        //    string arg = @$"-y -i {videoPath} -vframes 1 {thumbPath}";
        //    Console.WriteLine(arg);
        //    var ffmpeg = new ProcessStartInfo
        //    {
        //        FileName = ffmpegPath,
        //        Arguments = arg,
        //        WorkingDirectory = _env.WebRootPath,
        //        CreateNoWindow = true,
        //        UseShellExecute = true
        //    };

        //    using (var process = new Process { StartInfo = ffmpeg })
        //    {
        //        process.Start();
        //        process.WaitForExit();
        //    }
        //}
    }
}