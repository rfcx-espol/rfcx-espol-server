using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;



namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AlertaConfiguracionController
    {
        
        private readonly IAlertaConfiguracionRepository _AlertaConfiguracionRepository;

        public AlertaConfiguracionController(IAlertaConfiguracionRepository AlertaConfiguracionRepository)
        {
            _AlertaConfiguracionRepository=AlertaConfiguracionRepository;

        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetAlertaConfiguracion();
        }

        public async Task<string> GetAlertaConfiguracion()
        {
            var AlertaConfiguracions= await _AlertaConfiguracionRepository.Get();
            return JsonConvert.SerializeObject(AlertaConfiguracions);
        }


        [HttpGet]
        public Task<string> Get(string id)
        {
            return this.GetAlertaConfiguracionById(id);
        }

        public async Task<string> GetAlertaConfiguracionById(string id)
        {
            var AlertaConfiguracion= await _AlertaConfiguracionRepository.Get(id) ?? new AlertaConfiguracion();
            return JsonConvert.SerializeObject(AlertaConfiguracion);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] AlertaConfiguracion AlertaConfiguracion)
        {
            await _AlertaConfiguracionRepository.Add(AlertaConfiguracion);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<string> Put(string id, [FromBody] AlertaConfiguracion AlertaConfiguracion)
        {
            if (string.isNullOrEmpty(id)) return "Id no válida";
            return await _AlertaConfiguracionRepository.Update(id, AlertaConfiguracion);
        }

        [HttpDelete("{id}")]
        public async Task<string> Delete(string id)
        {
            if (string.isNullOrEmpty(id)) return "Id no válida";
            await _AlertaConfiguracionRepository.Remove(id);
            return "";
        }
    }
}