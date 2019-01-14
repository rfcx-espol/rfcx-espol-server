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
        public async Task<ActionResult> Show(int id)
        {
            var image = await Image.Find(id);
            return base.PhysicalFile(image.Ruta, "image/jpeg");
        }
        [HttpPost]
        public ActionResult PostPicture([FromBody] ImageRequest req)
        {
            
            Image.PostPicture(req.FechaCaptura, req.IdEstacion, req.Base64Image);
            return new OkResult();
        }
    }
}