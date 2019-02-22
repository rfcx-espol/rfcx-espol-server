using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;
using System.Web;
using System.IO;
using System.Collections.Generic;

namespace WebApplication.Controllers
{
    [Route("api/imgcapture")]
    public class ImageController : Controller
    {
        private readonly IImageRepository _ImageRepository;

        public ImageController(IImageRepository ImageRepository)
        {
            _ImageRepository=ImageRepository;

        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Image> model = _ImageRepository.GetAllProducts().Result;
            return View("Index", model);
        }

        [HttpGet("{_id}")]
        public async Task<ActionResult> Show(string _id)
        {
            var image = await _ImageRepository.Find(_id);
            var imgPath = Constants.RUTA_ARCHIVOS_ANALISIS_IMAGENES + image.StationId + "/" +  image.Path;
            return base.PhysicalFile(imgPath, "image/"+Path.GetExtension(image.Path).Substring(1));
        }

        [HttpPost]
        public async Task<ActionResult> PostPicture(ImageRequest req)
        {
            return await _ImageRepository.PostPicture(req);
            
        }

        [HttpGet("list")]
        public async Task<ActionResult> List([FromQuery]int stationid,[FromQuery]long starttime, [FromQuery]long endtime, [FromQuery]int page=1, [FromQuery]int rows=25)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime start = starttime == 0? DateTime.Today : epoch.AddSeconds(starttime);
            DateTime end = endtime == 0? DateTime.Today.AddDays(1) : epoch.AddSeconds(endtime);
            var arr = await _ImageRepository.ListImages(start, end, page, rows, stationid);
            return new ContentResult(){ Content = JsonConvert.SerializeObject(arr)};
        }
    }
}