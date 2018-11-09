using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;

namespace WebApplication.Controllers
{
    [Route("")]
    public class KindController
    {
        
        private readonly IKindRepository _KindRepository;

        public KindController(IKindRepository KindRepository)
        {
            _KindRepository=KindRepository;
        }

        [HttpGet]
        [Route("api/bpv/[controller]")]
        public Task<string> Get()
        {
            return this.GetKind();
        }

        private async Task<string> GetKind()
        {
            var Kind= await _KindRepository.Get();
            return JsonConvert.SerializeObject(Kind);
        }

        [HttpGet]
        [Route("api/bpv/[controller]/{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetKindByIdInt(id);
        }

        private async Task<string> GetKindByIdInt(int id)
        {
            var Kind= await _KindRepository.Get(id) ?? new Kind();
            return JsonConvert.SerializeObject(Kind);
        }

        [HttpPut]
        [Route("api/bpv/[controller]/{KindId:int}")]
        public async Task<bool> Put([FromRoute]int KindId, [FromBody] Kind Kind)
        {
            if (KindId==0) return false;
            return await _KindRepository.Update(KindId, Kind);
        }

        [HttpDelete]
        [Route("api/bpv/[controller]/{KindId:int}")]
        public async Task<bool> Delete([FromRoute]int KindId)
        {
            if (KindId==0) return false;
            return await _KindRepository.Remove(KindId);
        }

    }

}