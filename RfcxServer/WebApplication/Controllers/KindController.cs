using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;

namespace WebApplication.Controllers
{
    [Route("api/bpv/[controller]")]
    public class KindController
    {
        
        private readonly IKindRepository _KindRepository;

        public KindController(IKindRepository KindRepository)
        {
            _KindRepository=KindRepository;
        }

        [HttpGet()]
        public Task<string> Get()
        {
            return this.GetKind();
        }

        private async Task<string> GetKind()
        {
            var Kind= await _KindRepository.Get();
            return JsonConvert.SerializeObject(Kind);
        }

        [HttpGet("{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetKindByIdInt(id);
        }

        private async Task<string> GetKindByIdInt(int id)
        {
            var Kind= await _KindRepository.Get(id) ?? new Kind();
            return JsonConvert.SerializeObject(Kind);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Kind Kind)
        {            
            var x = await _KindRepository.Add(Kind);
            if(x==false){
                return "Id already exists!";
            }
            return "";
        }

        [HttpPatch("{id}/Name")]
        public async Task<bool> PatchName(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _KindRepository.UpdateName(id, json.Name);
        }

        [HttpPatch("{id}/Family")]
        public async Task<bool> PatchFamily(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _KindRepository.UpdateFamily(id, json.Family);
        }

        [HttpDelete("{KindId:int}")]
        public async Task<bool> Delete([FromRoute]int KindId)
        {
            if (KindId==0) return false;
            return await _KindRepository.Remove(KindId);
        }

    }

}