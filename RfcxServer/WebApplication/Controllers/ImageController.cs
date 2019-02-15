using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;
using System.Web;
using System.IO;
using System.Threading.Tasks;

namespace WebApplication.Controllers
{
    [Route("api/imgcapture")]
    public class ImageController : ControllerBase
    {
        [HttpGet("{_id}")]
        public async Task<ActionResult> Show(string _id)
        {
            var image = await Image.Find(_id);
            var imgPath = Constants.RUTA_ARCHIVOS_ANALISIS_IMAGENES + image.StationId + "/" +  image.Path;
            return base.PhysicalFile(imgPath, "image/"+Path.GetExtension(image.Path).Substring(1));
        }
        [HttpPost]
        public async Task<ActionResult> PostPicture(ImageRequest req)
        {
            return await Image.PostPicture(req);
            
        }
        [HttpGet("list")]
        public async Task<ActionResult> List([FromQueryAttribute]long starttime, [FromQueryAttribute]long endtime, [FromQueryAttribute]int page=1, [FromQueryAttribute]int rows=1)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime start = epoch.AddSeconds(starttime);
            DateTime end = epoch.AddSeconds(endtime);
            var arr = await Image.ListImages(start, end, page, rows);
            return new ContentResult(){ Content = JsonConvert.SerializeObject(arr)};
        }
    }
}