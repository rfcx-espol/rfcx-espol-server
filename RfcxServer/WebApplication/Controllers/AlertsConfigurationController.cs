using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;



namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AlertsConfigurationController
    {
        
        private readonly IAlertsConfigurationRepository _AlertsConfigurationRepository;

        public AlertsConfigurationController(IAlertsConfigurationRepository AlertsConfigurationRepository)
        {
            _AlertsConfigurationRepository=AlertsConfigurationRepository;

        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetAlertsConfiguration();
        }

        public async Task<string> GetAlertsConfiguration()
        {
            var AlertsConfigurations= await _AlertsConfigurationRepository.Get();
            return JsonConvert.SerializeObject(AlertsConfigurations);
        }


        [HttpGet]
        public Task<string> Get(string id)
        {
            return this.GetAlertsConfigurationById(id);
        }

        public async Task<string> GetAlertsConfigurationById(string id)
        {
            var AlertsConfiguration= await _AlertsConfigurationRepository.Get(id) ?? new AlertsConfiguration();
            return JsonConvert.SerializeObject(AlertsConfiguration);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] AlertsConfiguration AlertsConfiguration)
        {
            await _AlertsConfigurationRepository.Add(AlertsConfiguration);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] AlertsConfiguration AlertsConfiguration)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _AlertsConfigurationRepository.Update(id, AlertsConfiguration);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _AlertsConfigurationRepository.Remove(id);
        }
    }
}