using Microsoft.AspNetCore.Mvc;
using ASPdeFish.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ASPdeFish.Controllers
{
    public class ThumbController : HomeController
    {
        public ThumbController(IWebHostEnvironment env) : base(env)
        {
            _env = env;
        }

        [HttpPost]
        public IActionResult ChangeImage(string factor)
        {
            double f = Convert.ToDouble(factor.Replace(".", ","));

            string copyName = "thumbcopy.png";
            string copyPath = Path.Combine(_env.WebRootPath, "images", copyName);


            if (HttpContext.Session != null)
            {
                videoModel = new Video(HttpContext.Session.GetString("thumbName"),
                                        HttpContext.Session.GetString("videoName"));
            }

            if (videoModel.ThumbName == null)
            {
                return View("../Home/Index");
            }

            string thumbPath = Path.Combine(_env.WebRootPath, "images", videoModel.ThumbName);

            DeFishEyeAlgorithm.deFishEye(thumbPath, f, copyPath);

            videoModel.ThumbName = copyName;
            videoModel.Factor = factor;

            return View("../Home/Index", videoModel);
        }
    }
}
