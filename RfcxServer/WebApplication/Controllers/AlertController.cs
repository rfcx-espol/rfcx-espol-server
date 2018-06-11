using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;



namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AlertController
    {
        
        private readonly IAlertRepository _AlertRepository;

        public AlertController(IAlertRepository AlertRepository)
        {
            _AlertRepository=AlertRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetAlert();
        }

        public async Task<string> GetAlert()
        {
            var Alerts= await _AlertRepository.Get();
            return JsonConvert.SerializeObject(Alerts);
        }


        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            return this.GetAlertById(id);
        }

        public async Task<string> GetAlertById(string id)
        {
            var Alert= await _AlertRepository.Get(id) ?? new Alert();
            return JsonConvert.SerializeObject(Alert);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Alert Alert)
        {
            await _AlertRepository.Add(Alert);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] Alert Alert)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _AlertRepository.Update(id, Alert);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _AlertRepository.Remove(id);
        }
    }
}