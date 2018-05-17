using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;


namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class DispositivoController
    {
        
        private readonly IDispositivoRepository _DispositivoRepository;

        public DispositivoController(IDispositivoRepository DispositivoRepository)
        {
            _DispositivoRepository=DispositivoRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetDispositivo();
        }

        public async Task<string> GetDispositivo()
        {
            var Dispositivos= await _DispositivoRepository.Get();
            return JsonConvert.SerializeObject(Dispositivos);
        }


        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            return this.GetDispositivoById(id);
        }

        public async Task<string> GetDispositivoById(string id)
        {
            var Dispositivo= await _DispositivoRepository.Get(id) ?? new Dispositivo();
            return JsonConvert.SerializeObject(Dispositivo);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Dispositivo Dispositivo)
        {
            await _DispositivoRepository.Add(Dispositivo);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] Dispositivo Dispositivo)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _DispositivoRepository.Update(id, Dispositivo);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _DispositivoRepository.Remove(id);
             
        }
    }
}