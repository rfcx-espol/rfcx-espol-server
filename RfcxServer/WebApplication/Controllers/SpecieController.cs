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
using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
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

        [HttpGet("index")]
        public IActionResult Index() {
            if(TempData["creacion"] == null)
                TempData["creacion"] = 0;
            if(TempData["eliminacion"] == null)
                TempData["eliminacion"] = 0;
            if(TempData["edicion"] == null)
                TempData["edicion"] = 0;
            List<Specie> especies = _SpecieRepository.Get();
            ViewBag.especies = especies;
            return View();
        }

        [HttpGet("create")]
        public IActionResult Create() {
            return View();
        }

        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id) {
            ViewBag.especie = _SpecieRepository.Get(id);
            DirectoryInfo DI = new DirectoryInfo(Constants.RUTA_ARCHIVOS_IMAGENES_ESPECIES + id.ToString() + "/");
            List<string> archivos = new List<string>();
            foreach (var file in DI.GetFiles())
            {
                archivos.Add(file.Name);
            }
            ViewBag.archivos = archivos;
            return View();
        }

        [HttpGet("list")]
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
        public string Get(int id)
        {
            return this.GetSpecieByIdInt(id);
        }

        private string GetSpecieByIdInt(int id)
        {
            var Specie= _SpecieRepository.Get(id) ?? new Specie();
            return JsonConvert.SerializeObject(Specie);
        }

        [HttpGet()]
        public Task<string> Get([FromQuery] string name)
        {
            return this.GetSpecieByName(name);
        }

        private async Task<string> GetSpecieByName(string name)
        {
            var Specie= await _SpecieRepository.GetSpecie(name) ?? new Specie();
            return JsonConvert.SerializeObject(Specie);
        }

        [HttpGet("{specieId:int}/gallery/{photoId:int}")]
        public ActionResult Get(int specieId, int photoId)
        {
            DirectoryInfo DI = new DirectoryInfo(Constants.RUTA_ARCHIVOS_IMAGENES_ESPECIES + specieId.ToString() + "/");
            foreach (var file in DI.GetFiles())
            {
                string[] extension = (file.Name).Split('.');
                if(extension[0] == photoId.ToString()) {
                    string fileAddress = DI.FullName + file.Name;
                    var net = new System.Net.WebClient();
                    var data = net.DownloadData(fileAddress);
                    var content = new System.IO.MemoryStream(data);
                    if(extension[1] == "jpg" || extension[1] == "jpeg") {
                        return File(content, "image/jpeg", file.Name);
                    } else {
                        return File(content, "image/png", file.Name);
                    }
                }
            }     
            return null;       
        }

        [HttpPost]
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
                _SpecieRepository.AddPhoto(spe.Id, photo);
                string[] extension = (archivos[i - 1].FileName).Split('.');
                filePath = Path.Combine(Core.SpecieFolderPath(spe.Id.ToString()), photo.Id.ToString() + "." + extension[1]);
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
                TempData["creacion"] = 1;
            else
                TempData["creacion"] = -1;           

            return Redirect("/api/bpv/specie/index/");
        }

        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            bool valor = _SpecieRepository.Remove(id);
            if(valor == true) {
                TempData["eliminacion"] = 1;
            } else {
                TempData["eliminacion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Name")]
        public bool PatchName(int id, [FromBody] Arrays json)
        {
            bool valor = _SpecieRepository.UpdateName(id, json.Name);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Family")]
        public bool PatchFamily(int id, [FromBody] Arrays json)
        {
            bool valor = _SpecieRepository.UpdateFamily(id, json.Family);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Gallery")]
        public bool PatchGallery(int id, [FromBody] Arrays json)
        {
            bool valor = _SpecieRepository.UpdatePhoto(id, json.IdPhoto, json.Description);
            valor = _PhotoRepository.UpdateDescription(json.IdPhoto, json.Description);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPost("{id:int}/addPhotos")]
        public async Task<IActionResult> addPhotos(int id, List<string> descripciones, List<IFormFile> archivos)
        {
            string filePath;
            Task result = null;
            Specie especie = _SpecieRepository.Get(id);

            for(int i = 1; i < (archivos.Count + 1); i++)
            {
                Photo photo = new Photo();
                photo.Description = descripciones[i - 1];
                result = _PhotoRepository.Add(photo);
                _SpecieRepository.AddPhoto(especie.Id, photo);
                string[] extension = (archivos[i - 1].FileName).Split('.');
                filePath = Path.Combine(Core.SpecieFolderPath(especie.Id.ToString()), photo.Id.ToString() + "." + extension[1]);
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
                TempData["edicion"] = 1;
            else
                TempData["edicion"] = -1;           

            return Redirect("/api/bpv/specie/index/");
        }

    }

}