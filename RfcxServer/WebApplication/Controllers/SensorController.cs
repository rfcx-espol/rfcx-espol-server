using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;


namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class SensorController
    {
        
        private readonly ISensorRepository _SensorRepository;

        public SensorController(ISensorRepository SensorRepository)
        {
            _SensorRepository=SensorRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetSensor();
        }

        public async Task<string> GetSensor()
        {
            var Sensors= await _SensorRepository.Get();
            return JsonConvert.SerializeObject(Sensors);
        }


        [HttpGet]
        public Task<string> Get(string id)
        {
            return this.GetSensorById(id);
        }

        public async Task<string> GetSensorById(string id)
        {
            var Sensor= await _SensorRepository.Get(id) ?? new Sensor();
            return JsonConvert.SerializeObject(Sensor);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Sensor Sensor)
        {
            await _SensorRepository.Add(Sensor);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] Sensor Sensor)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _SensorRepository.Update(id, Sensor);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _SensorRepository.Remove(id);
        }
    }
}