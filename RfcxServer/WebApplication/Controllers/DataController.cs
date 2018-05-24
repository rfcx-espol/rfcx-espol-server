using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;



namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class DataController
    {
        
        private readonly IDataRepository _DataRepository;

        public DataController(IDataRepository DataRepository)
        {
            _DataRepository=DataRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetData();
        }

        public async Task<string> GetData()
        {
            var Datas= await _DataRepository.Get();
            return JsonConvert.SerializeObject(Datas);
        }


        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            return this.GetDataById(id);
        }

        public async Task<string> GetDataById(string id)
        {
            var Data= await _DataRepository.Get(id) ?? new Data();
            return JsonConvert.SerializeObject(Data);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Data Data)
        {
            await _DataRepository.Add(Data);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] Data Data)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _DataRepository.Update(id, Data);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _DataRepository.Remove(id);
        }
    }
}