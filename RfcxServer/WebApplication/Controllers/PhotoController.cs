using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;

namespace WebApplication.Controllers
{
    [Route("")]
    public class PhotoController
    {
        
        private readonly IPhotoRepository _PhotoRepository;

        public PhotoController(IPhotoRepository PhotoRepository)
        {
            _PhotoRepository=PhotoRepository;
        }

        [HttpGet("api/bpv/[controller]")]
        public Task<string> Get()
        {
            return this.GetPhoto();
        }

        private async Task<string> GetPhoto()
        {
            var Photo= await _PhotoRepository.Get();
            return JsonConvert.SerializeObject(Photo);
        }

        [HttpGet("api/bpv/[controller]/{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetPhotoByIdInt(id);
        }

        private async Task<string> GetPhotoByIdInt(int id)
        {
            var Photo= await _PhotoRepository.Get(id) ?? new Photo();
            return JsonConvert.SerializeObject(Photo);
        }

        [HttpGet("api/bpv/Specie/{SpecieId:int}/[controller]")]
        public Task<string> GetPhotosBySpecie([FromRoute]int SpecieId)
        {
            return this.GetPhotoBySpecie(SpecieId);
        }

        private async Task<string> GetPhotoBySpecie(int SpecieId)
        {
            var Photos= await _PhotoRepository.GetBySpecie(SpecieId);
            return JsonConvert.SerializeObject(Photos);
        }

        [HttpGet("api/bpv/Specie/{SpecieId:int}/[controller]/{PhotoId:int}")]
        public Task<string> Get([FromRoute]int SpecieId, [FromRoute]int PhotoId)
        {
            return this.GetPhotoById(SpecieId, PhotoId);
        }

        private async Task<string> GetPhotoById(int SpecieId, int PhotoId)
        {
            var Photo= await _PhotoRepository.Get(SpecieId, PhotoId) ?? new Photo();
            return JsonConvert.SerializeObject(Photo);
        }

        [HttpDelete("api/bpv/[controller]/{PhotoId:int}")]
        public async Task<bool> Delete([FromRoute]int PhotoId)
        {
            if (PhotoId==0) return false;
            return await _PhotoRepository.Remove(PhotoId);
        }

    }

}