using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;


namespace WebApplication.Controllers
{
    [Route("")]
    public class SensorController
    {
        
        private readonly ISensorRepository _SensorRepository;

        public SensorController(ISensorRepository SensorRepository)
        {
            _SensorRepository=SensorRepository;
        }

        [HttpGet]
        [Route("api/[controller]")]
        public Task<string> Get()
        {
            return this.GetSensor();
        }

        private async Task<string> GetSensor()
        {
            var Sensors= await _SensorRepository.Get();
            return JsonConvert.SerializeObject(Sensors);
        }

        /* */
        [HttpGet]
        [Route("api/[controller]/{id}")]
        public Task<string> Get(string id)
        {
            return this.IdString(id);
        }

        private async Task<string> IdString(string id)
        {
            var Sensor= await _SensorRepository.Get(id) ?? new Sensor();
            return JsonConvert.SerializeObject(Sensor);
        }
        
        [HttpGet]
        [Route("api/[controller]/{id:int}")]
        public Task<string> Get(int id)
        {
            return this.IntId(id);
        }

        private async Task<string> IntId(int id)
        {
            var Sensor= await _SensorRepository.Get(id) ?? new Sensor();
            return JsonConvert.SerializeObject(Sensor);
        }

        [HttpGet]
        [Route("api/Device/{DeviceId:int}/[controller]")]
        public Task<string> GetSensorsByDevice([FromRoute]int DeviceId)
        {
            return this.GetSensorByDevice(DeviceId);
        }

        private async Task<string> GetSensorByDevice(int DeviceId)
        {
            var Sensors= await _SensorRepository.GetByDevice(DeviceId);
            return JsonConvert.SerializeObject(Sensors);
        }


        [HttpGet]
        [Route("api/Device/{DeviceId:int}/[controller]/{SensorId:int}")]
        public Task<string> Get([FromRoute]int DeviceId, [FromRoute]int SensorId)
        {
            return this.GetSensorById(DeviceId,SensorId);
        }

        private async Task<string> GetSensorById(int DeviceId, int SensorId)
        {
            var Sensor= await _SensorRepository.Get(DeviceId, SensorId) ?? new Sensor();
            return JsonConvert.SerializeObject(Sensor);
        }

        /*
        [HttpPost]
        [Route("api/[controller]")]
        public async Task<string> Post([FromBody] Sensor Sensor)
        {
            await _SensorRepository.Add(Sensor);
            return "";
        }
        */

        [HttpPut]
        [Route("api/[controller]/{id}")]
        public async Task<bool> Put(string id, [FromBody] Sensor Sensor)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _SensorRepository.Update(id, Sensor);
        }

        [HttpDelete]
        [Route("api/[controller]/{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _SensorRepository.Remove(id);
        }
    }
}