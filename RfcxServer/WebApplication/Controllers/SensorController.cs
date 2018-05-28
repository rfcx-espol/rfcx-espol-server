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

        public async Task<string> GetSensor()
        {
            var Sensors= await _SensorRepository.Get();
            return JsonConvert.SerializeObject(Sensors);
        }


        [HttpGet]
        [Route("api/[controller]/{id}")]
        public Task<string> Get(string id)
        {
            return this.GetSensorById(id);
        }

        public async Task<string> GetSensorById(string id)
        {
            var Sensor= await _SensorRepository.Get(id) ?? new Sensor();
            return JsonConvert.SerializeObject(Sensor);
        }

        [HttpGet]
        [Route("api/[controller]/{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetSensorById(id);
        }

        public async Task<string> GetSensorById(int id)
        {
            var Sensor= await _SensorRepository.Get(id) ?? new Sensor();
            return JsonConvert.SerializeObject(Sensor);
        }

        [HttpGet]
        [Route("api/Dispositivo/{DispositivoId:int}/[controller]")]
        public Task<string> GetSensorsByDispositivo([FromRoute]int DispositivoId)
        {
            return this.GetSensorByDispositivo(DispositivoId);
        }

        public async Task<string> GetSensorByDispositivo(int DispositivoId)
        {
            var Sensors= await _SensorRepository.GetByDispositivo(DispositivoId);
            return JsonConvert.SerializeObject(Sensors);
        }


        [HttpGet]
        [Route("api/Dispositivo/{DispositivoId:int}/[controller]/{SensorId:int}")]
        public Task<string> Get([FromRoute]int DispositivoId, [FromRoute]int SensorId)
        {
            return this.GetSensorById(DispositivoId,SensorId);
        }

        public async Task<string> GetSensorById(int DispositivoId, int SensorId)
        {
            var Sensor= await _SensorRepository.Get(DispositivoId, SensorId) ?? new Sensor();
            return JsonConvert.SerializeObject(Sensor);
        }


        [HttpPost]
        [Route("api/[controller]")]
        public async Task<string> Post([FromBody] Sensor Sensor)
        {
            await _SensorRepository.Add(Sensor);
            return "";
        }

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