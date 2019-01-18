using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Threading.Tasks;


namespace WebApplication.Controllers
{
    [Route("api/imgcapture")]
    public class ImageController : ControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Show(string id)
        {
            var image = await Image.Find(id);
            return base.PhysicalFile(image.Ruta, "image/jpeg");
        }
        [HttpPost]
        public async Task<ActionResult> PostPicture(ImageRequest req)
        {
            await Image.PostPicture(req);
            return new OkResult();
        }
    }
}