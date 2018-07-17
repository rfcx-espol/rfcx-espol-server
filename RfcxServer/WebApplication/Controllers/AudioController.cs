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
        [Route("api/Dispositivo/{DispositivoId:int}/[controller]")]
        public Task<string> GetAudiosByDispositivo([FromRoute]int DispositivoId)
        {
            return this.GetAudioByDispositivo(DispositivoId);
        }

        private async Task<string> GetAudioByDispositivo(int DispositivoId)
        {
            var Audios= await _AudioRepository.GetByDispositivo(DispositivoId);
            return JsonConvert.SerializeObject(Audios);
        }


        //[HttpGet("{id}")]
        [HttpGet]
        [Route("api/Dispositivo/{DispositivoId:int}/[controller]/{AudioId:int}")]
        //[Route("")]
        public Task<string> Get([FromRoute]int DispositivoId, [FromRoute]int AudioId)
        {
            return this.GetAudioById(DispositivoId, AudioId);
        }

        private async Task<string> GetAudioById(int DispositivoId, int AudioId)
        {
            var Audio= await _AudioRepository.Get(DispositivoId, AudioId) ?? new Audio();
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
        [Route("api/Dispositivo/{DispositivoId:int}/[controller]/{AudioId:int}")]
        public async Task<bool> Put([FromRoute]int DispositivoId, [FromRoute]int AudioId, [FromBody] Audio Audio)
        {
            if (AudioId==0) return false;
            return await _AudioRepository.Update(DispositivoId, AudioId, Audio);
        }

        //[HttpDelete("{id}")]
        [HttpDelete]
        [Route("api/Dispositivo/{DispositivoId:int}/[controller]/{AudioId:int}")]
        public async Task<bool> Delete([FromRoute]int DispositivoId, [FromRoute]int AudioId)
        {
            if (AudioId==0) return false;
            return await _AudioRepository.Remove(DispositivoId, AudioId);
        }
    }
}