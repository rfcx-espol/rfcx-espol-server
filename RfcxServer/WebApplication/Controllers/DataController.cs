using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;



namespace WebApplication.Controllers
{
    [Route("")]
    public class DataController
    {
        
        private readonly IDataRepository _DataRepository;

        public DataController(IDataRepository DataRepository)
        {
            _DataRepository=DataRepository;
        }

        [HttpGet]
        [Route("api/[controller]")]
        public Task<string> Get()
        {
            return this.GetData();
        }

        private async Task<string> GetData()
        {
            var Datas= await _DataRepository.Get();
            return JsonConvert.SerializeObject(Datas);
        }

        /*
        [HttpGet]
        [Route("api/[controller]/{id}")]
        public Task<string> Get(string id)
        {
            return this.GetDataById(id);
        }

        public async Task<string> GetDataById(string id)
        {
            var Data= await _DataRepository.Get(id) ?? new Data();
            return JsonConvert.SerializeObject(Data);
        }

        */
        [HttpGet]
        [Route("api/[controller]/{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetDataByIdInt(id);
        }

        private async Task<string> GetDataByIdInt(int id)
        {
            var Data= await _DataRepository.Get(id) ?? new Data();
            return JsonConvert.SerializeObject(Data);
        }

        [HttpGet]
        [Route("api/Dispositivo/{DispositivoId:int}/[controller]")]
        public Task<string> GetDatasByDispositivo([FromRoute]int DispositivoId)
        {
            return this.GetDataByDispositivo(DispositivoId);
        }

        private async Task<string> GetDataByDispositivo(int DispositivoId)
        {
            var Datass= await _DataRepository.GetByDispositivo(DispositivoId);
            return JsonConvert.SerializeObject(Datass);
        }

        [HttpGet]
        [Route("api/Dispositivo/{DispositivoId:int}/Sensor/{SensorId:int}/[controller]")]
        public Task<string> GetDatasByDispositivoSensor([FromRoute]int DispositivoId,[FromRoute] int SensorId)
        {
            return this.GetDataByDispositivoSensor(DispositivoId, SensorId);
        }

        private async Task<string> GetDataByDispositivoSensor(int DispositivoId, int SensorId)
        {
            var Datas= await _DataRepository.GetByDispositivoSensor(DispositivoId, SensorId);
            return JsonConvert.SerializeObject(Datas);
        }



        [HttpGet]
        [Route("api/Dispositivo/{DispositivoId:int}/Sensor/{SensorId:int}/[controller]/{DataId:int}")]
        public Task<string> Get([FromRoute]int DispositivoId, [FromRoute]int SensorId, [FromRoute]int DataId)
        {
            return this.GetDataById(DispositivoId, SensorId, DataId);
        }

        private async Task<string> GetDataById(int DispositivoId, int SensorId, int DataId)
        {
            var Data= await _DataRepository.Get(DispositivoId, SensorId, DataId) ?? new Data();
            return JsonConvert.SerializeObject(Data);
        }
        
        [HttpPost]
        [Route("api/[controller]")]
        public async Task<string> Post([FromBody] List<Data> DataList)
        {
            
            for (var i = 0; i < DataList.Count; i++) {
                _DataRepository.Add(DataList[i]);
            }
            /* 
             await _DataRepository.Add(Data);
            */
            return "";
        }

        [HttpPut]
        [Route("api/[controller]/{id}")]
        public async Task<bool> Put(string id, [FromBody] Data Data)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _DataRepository.Update(id, Data);
        }

        [HttpDelete]
        [Route("api/[controller]/{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _DataRepository.Remove(id);
        }
    }
}