using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;



namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AlertaController
    {
        
        private readonly IAlertaRepository _alertaRepository;

        public AlertaController(IAlertaRepository alertaRepository)
        {
            _alertaRepository=alertaRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetAlerta();
        }

        public async Task<string> GetAlerta()
        {
            var alertas= await _alertaRepository.Get();
            return JsonConvert.SerializeObject(alertas);
        }


        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            return this.GetAlertaById(id);
        }

        public async Task<string> GetAlertaById(string id)
        {
            var alerta= await _alertaRepository.Get(id) ?? new Alerta();
            return JsonConvert.SerializeObject(alerta);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Alerta alerta)
        {
            await _alertaRepository.Add(alerta);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] Alerta alerta)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _alertaRepository.Update(id, alerta);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _alertaRepository.Remove(id);
        }
    }
}