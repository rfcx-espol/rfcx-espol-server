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

        [HttpGet]
        [Route("api/bpv/[controller]")]
        public Task<string> Get()
        {
            return this.GetPhoto();
        }

        private async Task<string> GetPhoto()
        {
            var Photo= await _PhotoRepository.Get();
            return JsonConvert.SerializeObject(Photo);
        }

        [HttpGet]
        [Route("api/bpv/[controller]/{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetPhotoByIdInt(id);
        }

        private async Task<string> GetPhotoByIdInt(int id)
        {
            var Photo= await _PhotoRepository.Get(id) ?? new Photo();
            return JsonConvert.SerializeObject(Photo);
        }

        [HttpGet]
        [Route("api/bpv/Kind/{KindId:int}/[controller]")]
        public Task<string> GetPhotosByKind([FromRoute]int KindId)
        {
            return this.GetPhotoByKind(KindId);
        }

        private async Task<string> GetPhotoByKind(int KindId)
        {
            var Photos= await _PhotoRepository.GetByKind(KindId);
            return JsonConvert.SerializeObject(Photos);
        }

        [HttpGet]
        [Route("api/bpv/Kind/{KindId:int}/[controller]/{PhotoId:int}")]
        public Task<string> Get([FromRoute]int KindId, [FromRoute]int PhotoId)
        {
            return this.GetPhotoById(KindId, PhotoId);
        }

        private async Task<string> GetPhotoById(int KindId, int PhotoId)
        {
            var Photo= await _PhotoRepository.Get(KindId, PhotoId) ?? new Photo();
            return JsonConvert.SerializeObject(Photo);
        }

        [HttpPut]
        [Route("api/bpv/[controller]/{PhotoId:int}")]
        public async Task<bool> Put([FromRoute]int PhotoId, [FromBody] Photo Photo)
        {
            if (PhotoId==0) return false;
            return await _PhotoRepository.Update(PhotoId, Photo);
        }

        [HttpDelete]
        [Route("api/bpv/[controller]/{PhotoId:int}")]
        public async Task<bool> Delete([FromRoute]int PhotoId)
        {
            if (PhotoId==0) return false;
            return await _PhotoRepository.Remove(PhotoId);
        }

    }

}