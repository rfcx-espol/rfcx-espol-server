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

namespace WebApplication.Controllers
{
    [Route("api/bpv/[controller]")]
    public class SpecieController
    {
        
        private readonly ISpecieRepository _SpecieRepository;

        public SpecieController(ISpecieRepository SpecieRepository)
        {
            _SpecieRepository=SpecieRepository;
        }

        [HttpGet()]
        public Task<string> Get()
        {
            return this.GetSpecie();
        }

        private async Task<string> GetSpecie()
        {
            var Specie= await _SpecieRepository.Get();
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

        [HttpGet("{specieId:int}/Photo/{photoId:int}")]
        public Task<Photo> Get(int specieId, int photoId)
        {
            return this.GetPhotoByIdInt(specieId, photoId);
        }

        private async Task<Photo> GetPhotoByIdInt(int specieId, int photoId)
        {
            var filePath = await _SpecieRepository.GetPhoto(specieId, photoId);
            string url = Constants.BASE_URL + "api/bpv/Specie/" + specieId.ToString() + "/Photo/" + photoId.ToString();
            using (var client = new WebClient())
            { 
                client.DownloadFile(url, filePath);
                return null;
            }
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Specie Specie)
        {            
            var x = await _SpecieRepository.Add(Specie);
            if(x==false){
                return "Id already exists!";
            }
            return "";
        }

        [HttpPatch("{id}/Name")]
        public async Task<bool> PatchName(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _SpecieRepository.UpdateName(id, json.Name);
        }

        [HttpPatch("{id}/Family")]
        public async Task<bool> PatchFamily(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _SpecieRepository.UpdateFamily(id, json.Family);
        }

        [HttpDelete("{SpecieId:int}")]
        public async Task<bool> Delete([FromRoute]int SpecieId)
        {
            if (SpecieId==0) return false;
            return await _SpecieRepository.Remove(SpecieId);
        }

    }

}