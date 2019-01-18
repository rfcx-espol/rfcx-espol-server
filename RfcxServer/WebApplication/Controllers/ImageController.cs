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
            return base.PhysicalFile(image.Ruta, "image/"+Path.GetExtension(image.Ruta).Substring(1));
        }
        [HttpPost]
        public async Task<ActionResult> PostPicture(ImageRequest req)
        {
            await Image.PostPicture(req);
            return new OkResult();
        }
    }
}