using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;


namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class DeviceController
    {
        
        private readonly IDeviceRepository _DeviceRepository;

        public DeviceController(IDeviceRepository DeviceRepository)
        {
            _DeviceRepository=DeviceRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetDevice();
        }

        public async Task<string> GetDevice()
        {
            var Devices= await _DeviceRepository.Get();
            return JsonConvert.SerializeObject(Devices);
        }


        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            return this.GetDeviceById(id);
        }

        public async Task<string> GetDeviceById(string id)
        {
            var Device= await _DeviceRepository.Get(id) ?? new Device();
            return JsonConvert.SerializeObject(Device);
        }

        [HttpGet("{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetDeviceById(id);
        }

        public async Task<string> GetDeviceById(int id)
        {
            var Device= await _DeviceRepository.Get(id) ?? new Device();
            return JsonConvert.SerializeObject(Device);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Device Device)
        {
            var nombre=Device.Name;
            
            await _DeviceRepository.Add(Device);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] Device Device)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _DeviceRepository.Update(id, Device);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _DeviceRepository.Remove(id);
             
        }


        [HttpPatch("{id}/AndroidV")]
        public async Task<bool> PatchVersionAndroid(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _DeviceRepository.UpdateAndroidVersion(id, json.AndroidVersion);
        }

        [HttpPatch("{id}/ServicesV")]
        public async Task<bool> PatchVersionVersionServices(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _DeviceRepository.UpdateServicesVersion(id, json.ServicesVersion);
        }

        [HttpPatch("{id}/Name")]
        public async Task<bool> PatchName(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _DeviceRepository.UpdateName(id, json.Name);
        }

        [HttpPatch("{id}/Coordinates")]
        public async Task<bool> PatchPosition(int id, [FromBody]  Arrays json)
        {
            if (id==0) return false;
            return await _DeviceRepository.UpdatePosition(id, json.Latitude, json.Longitude);
        }

    }
}