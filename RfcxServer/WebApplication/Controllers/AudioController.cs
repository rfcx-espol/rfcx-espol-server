using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;


namespace WebApplication.Controllers
{
    [Route("")]
    public class AudioController
    {
        
        private readonly IAudioRepository _AudioRepository;

        public AudioController(IAudioRepository AudioRepository)
        {
            _AudioRepository=AudioRepository;
        }

        [HttpGet]
        [Route("api/[controller]")]
        public Task<string> Get()
        {
            return this.GetAudio();
        }

        private async Task<string> GetAudio()
        {
            var Audios= await _AudioRepository.Get();
            return JsonConvert.SerializeObject(Audios);
        }

        /*
        [HttpGet]
        [Route("api/[controller]/{id}")]
        public Task<string> Get(string id)
        {
            return this.GetAudioById(id);
        }

        public async Task<string> GetAudioById(string id)
        {
            var Audio= await _AudioRepository.Get(id) ?? new Audio();
            return JsonConvert.SerializeObject(Audio);
        }
        */

        [HttpGet]
        [Route("api/[controller]/{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetAudioByIdInt(id);
        }

        private async Task<string> GetAudioByIdInt(int id)
        {
            var Audio= await _AudioRepository.Get(id) ?? new Audio();
            return JsonConvert.SerializeObject(Audio);
        }
        /*
        [HttpGet("{id}")]
        public Task<string> Get(int id)
        {
            return this.GetAudioById(id);
        }

        public async Task<string> GetAudioById(int id)
        {
            var Audio= await _AudioRepository.Get(id) ?? new Audio();
            return JsonConvert.SerializeObject(Audio);
        }
        */

        [HttpGet]
        [Route("api/Station/{StationId:int}/[controller]")]
        public Task<string> GetAudiosByStation([FromRoute]int StationId)
        {
            return this.GetAudioByStation(StationId);
        }

        private async Task<string> GetAudioByStation(int StationId)
        {
            var Audios= await _AudioRepository.GetByStation(StationId);
            return JsonConvert.SerializeObject(Audios);
        }


        //[HttpGet("{id}")]
        [HttpGet]
        [Route("api/Station/{StationId:int}/[controller]/{AudioId:int}")]
        //[Route("")]
        public Task<string> Get([FromRoute]int StationId, [FromRoute]int AudioId)
        {
            return this.GetAudioById(StationId, AudioId);
        }

        private async Task<string> GetAudioById(int StationId, int AudioId)
        {
            var Audio= await _AudioRepository.Get(StationId, AudioId) ?? new Audio();
            return JsonConvert.SerializeObject(Audio);
        }
        /* 
        [HttpPost]
        [Route("api/[controller]")]
        public async Task<string> Post([FromBody] Audio Audio)
        {
            await _AudioRepository.Add(Audio);
            return "";
        }
        */


        //[HttpPut("{id}")]
        [HttpPut]
        [Route("api/Station/{StationId:int}/[controller]/{AudioId:int}")]
        public async Task<bool> Put([FromRoute]int StationId, [FromRoute]int AudioId, [FromBody] Audio Audio)
        {
            if (AudioId==0) return false;
            return await _AudioRepository.Update(StationId, AudioId, Audio);
        }

        //[HttpDelete("{id}")]
        [HttpDelete]
        [Route("Download/api/Station/{StationId:int}/[controller]/{AudioId:int}")]
        public async Task<bool> Delete([FromRoute]int StationId, [FromRoute]int AudioId)
        {
            if (AudioId==0) return false;
            return await _AudioRepository.Remove(StationId, AudioId);
        }
    }
}