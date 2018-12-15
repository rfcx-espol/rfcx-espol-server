using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Net;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Encodings.Web;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using WebApplication.Controllers;
using WebApplication.Repository;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Web;
using System.Drawing;
using System.Net.Http;

namespace WebApplication
{
    [Route("api/bpv/[controller]")]
    public class SpecieController : Controller
    {
        private readonly ISpecieRepository _SpecieRepository;
        private readonly IPhotoRepository _PhotoRepository;
        private readonly IFileProvider _fileProvider;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public SpecieController(IFileProvider fileProvider, ISpecieRepository SpecieRepository, IPhotoRepository PhotoRepository)
        {
            _fileProvider = fileProvider;
            _SpecieRepository=SpecieRepository;
            _PhotoRepository= PhotoRepository;
        }

        [HttpGet("create")]
        public IActionResult Index() {
            if(TempData["exito"] == null)
                TempData["exito"] = 0;
            return View();
        }

        [HttpGet()]
        public List<Specie> Get()
        {
            return _SpecieRepository.Get();
        }

        private async Task<string> GetSpecie()
        {
            var Specie= _SpecieRepository.Get();
            return JsonConvert.SerializeObject(Specie);
        }

        [HttpGet("{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetSpecieByIdInt(id);
        }

        private async Task<string> GetSpecieByIdInt(int id)
        {
            var Specie= await _SpecieRepository.Get(id) ?? new Specie();
            return JsonConvert.SerializeObject(Specie);
        }

        [HttpGet("{specieId:int}/gallery/{photoId:int}")]
        public ActionResult Get(int specieId, int photoId)
        {
            DirectoryInfo DI = new DirectoryInfo(Constants.RUTA_ARCHIVOS_IMAGENES_ESPECIES + specieId.ToString() + "/");
            string fileAddress = DI.FullName + photoId.ToString() + ".jpg";
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileAddress);
            var content = new System.IO.MemoryStream(data);
            return File(content, "application/jpg", photoId.ToString() + ".jpg");
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Post(string nombre_especie, string familia, List<string> descripciones, 
                                                List<IFormFile> archivos)
        {
            string filePath;
            Task result;

            Specie spe = new Specie();
            spe.Name = nombre_especie;
            spe.Family = familia;
            spe.Gallery = new List<Photo>();
            result = _SpecieRepository.Add(spe);

            Core.MakeSpecieFolder(spe.Id.ToString());

            for(int i = 1; i < (archivos.Count + 1); i++)
            {
                Photo photo = new Photo();
                photo.Description = descripciones[i - 1];
                _PhotoRepository.Add(photo);
                await _SpecieRepository.AddPhoto(spe.Id, photo);
                filePath = Path.Combine(Core.SpecieFolderPath(spe.Id.ToString()), i.ToString() + ".jpg");
                if (archivos[i - 1].Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await archivos[i - 1].CopyToAsync(stream);
                    }
                }
            }

            if(result.Status == TaskStatus.RanToCompletion || result.Status == TaskStatus.Running ||
                result.Status == TaskStatus.Created || result.Status == TaskStatus.WaitingToRun)
                TempData["exito"] = 1;
            else
                TempData["exito"] = -1;           

            return Redirect("/api/bpv/specie/create/");
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete([FromRoute] int id)
        {
            if (id==0) return false;
            return await _SpecieRepository.Remove(id);
        }

    }

}