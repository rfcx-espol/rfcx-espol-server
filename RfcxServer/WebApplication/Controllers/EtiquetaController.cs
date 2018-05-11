using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System;


namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class EtiquetaController
    {
        
        private readonly IEtiquetaRepository _EtiquetaRepository;

        public EtiquetaController(IEtiquetaRepository EtiquetaRepository)
        {
            _EtiquetaRepository=EtiquetaRepository;
        }

        [HttpGet]
        public Task<string> Get()
        {
            return this.GetEtiqueta();
        }

        public async Task<string> GetEtiqueta()
        {
            var Etiquetas= await _EtiquetaRepository.Get();
            return JsonConvert.SerializeObject(Etiquetas);
        }


        [HttpGet]
        public Task<string> Get(string id)
        {
            return this.GetEtiquetaById(id);
        }

        public async Task<string> GetEtiquetaById(string id)
        {
            var Etiqueta= await _EtiquetaRepository.Get(id) ?? new Etiqueta();
            return JsonConvert.SerializeObject(Etiqueta);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Etiqueta Etiqueta)
        {
            await _EtiquetaRepository.Add(Etiqueta);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<string> Put(string id, [FromBody] Etiqueta Etiqueta)
        {
            if (string.isNullOrEmpty(id)) return "Id no válida";
            return await _EtiquetaRepository.Update(id, Etiqueta);
        }

        [HttpDelete("{id}")]
        public async Task<string> Delete(string id)
        {
            if (string.isNullOrEmpty(id)) return "Id no válida";
            await _EtiquetaRepository.Remove(id);
            return "";
        }
    }
}