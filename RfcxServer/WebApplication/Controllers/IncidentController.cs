using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication.IRepository;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class IncidentController : Controller
    {
        private readonly IIncidentRepository _IncidentRepository;
        public IncidentController(IIncidentRepository IncidentRepository)
        {
            _IncidentRepository = IncidentRepository;
        }

        [HttpGet("list")]
        public async Task<string> GetAllIncidents()
        {
            var Alerts = await _IncidentRepository.GetAllIncidents();
            return JsonConvert.SerializeObject(Alerts);
        }


        [HttpGet("{id}")]
        public async Task<string> Get(string id)
        {
            var Incident = await _IncidentRepository.GetIncident(id) ?? new Incident();
            return JsonConvert.SerializeObject(Incident);
        }

        [HttpPost]
        public async Task Post([FromBody] Incident incident)
        {
            await _IncidentRepository.AddIncident(incident);
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] Incident incident)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _IncidentRepository.UpdateIncident(id, incident);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _IncidentRepository.RemoveIncident(id);
        }

        [HttpPatch("{id}")]
        public async Task<bool> UpdateStatus(string id, [FromBody] Boolean status)
        {
            return await _IncidentRepository.UpdateIncidentStatus(id, status);
        }
    }
}