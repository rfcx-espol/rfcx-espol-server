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
        
        [HttpGet]
        [Route("api/[controller]/lastData")]
        public Task<string> GetLasts()
        {
            return this.GetLast();
        }

         private async Task<string> GetLast()
        {
            var Datass= await _DataRepository.GetLasts();
            return JsonConvert.SerializeObject(Datass);
        }

        private async Task<string> GetDataByIdInt(int id)
        {
            var Data= await _DataRepository.Get(id) ?? new Data();
            return JsonConvert.SerializeObject(Data);
        }

        [HttpGet]
        [Route("api/Device/{DeviceId:int}/[controller]")]
        public Task<string> GetDatasByDevice([FromRoute]int DeviceId)
        {
            return this.GetDataByDevice(DeviceId);
        }

        private async Task<string> GetDataByDevice(int DeviceId)
        {
            var Datass= await _DataRepository.GetByDevice(DeviceId);
            return JsonConvert.SerializeObject(Datass);
        }

        [HttpGet]
        [Route("api/Device/{DeviceId:int}/Sensor/{SensorId:int}/[controller]")]
        public Task<string> GetDatasByDeviceSensor([FromRoute]int DeviceId,[FromRoute] int SensorId)
        {
            return this.GetDataByDeviceSensor(DeviceId, SensorId);
        }

        private async Task<string> GetDataByDeviceSensor(int DeviceId, int SensorId)
        {
            var Datas= await _DataRepository.GetByDeviceSensor(DeviceId, SensorId);
            return JsonConvert.SerializeObject(Datas);
        }

        [HttpGet]
        [Route("api/Device/{DeviceId:int}/Sensor/{SensorId:int}/[controller]/lastData")]
        public Task<string> GetLastDataByDeviceSensor([FromRoute]int DeviceId,[FromRoute] int SensorId)
        {
            return this.GetLastsDataByDeviceSensor(DeviceId, SensorId);
        }

        private async Task<string> GetLastsDataByDeviceSensor(int DeviceId, int SensorId)
        {
            var Datas= await _DataRepository.GetLastByDeviceSensor(DeviceId, SensorId);
            return JsonConvert.SerializeObject(Datas);
        }

        [HttpGet]
        [Route("api/Device/{DeviceId:int}/Sensor/{SensorId:int}/[controller]/{StartTimestamp:long}/{EndTimestamp:long}")]
        public Task<string> GetDatasByDeviceSensorTimestamp([FromRoute]int DeviceId,[FromRoute] int SensorId, 
        [FromRoute] long StartTimestamp, [FromRoute] long EndTimestamp)
        {
            return this.GetDataByDeviceSensorTimeStamp(DeviceId, SensorId, StartTimestamp, EndTimestamp);
        }

        private async Task<string> GetDataByDeviceSensorTimeStamp(int DeviceId, int SensorId, long StartTimestamp, long EndTimeStamp)
        {
            var Datas= await _DataRepository.GetByDeviceSensorTimestamp(DeviceId, SensorId, StartTimestamp, EndTimeStamp);
            return JsonConvert.SerializeObject(Datas);
        }



        [HttpGet]
        [Route("api/Device/{DeviceId:int}/Sensor/{SensorId:int}/[controller]/{DataId:int}")]
        public Task<string> Get([FromRoute]int DeviceId, [FromRoute]int SensorId, [FromRoute]int DataId)
        {
            return this.GetDataById(DeviceId, SensorId, DataId);
        }

        private async Task<string> GetDataById(int DeviceId, int SensorId, int DataId)
        {
            var Data= await _DataRepository.Get(DeviceId, SensorId, DataId) ?? new Data();
            return JsonConvert.SerializeObject(Data);
        }
        
        [HttpPost]
        [Route("api/[controller]")]
        public string Post([FromBody] Arrays Array)
        {
            List<Data> data=Array.Data;
            for (var i = 0; i <data.Count; i++) {
                _DataRepository.Add(data[i]);
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