using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class AlertController : Controller
    {

        private readonly IAlertRepository _AlertRepository;

        public AlertController(IAlertRepository AlertRepository)
        {
            _AlertRepository = AlertRepository;
        }

        [HttpGet("list")]
        public Task<string> Get()
        {
            return this.GetAlert();
        }

        private async Task<string> GetAlert()
        {
            var Alerts = await _AlertRepository.GetAllAlerts();
            return JsonConvert.SerializeObject(Alerts);
        }


        [HttpGet("{id}")]
        public Task<string> Get(string id)
        {
            return this.GetAlertById(id);
        }

        private async Task<string> GetAlertById(string id)
        {
            var Alert = await _AlertRepository.GetAlert(id) ?? new Alert();
            return JsonConvert.SerializeObject(Alert);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] Alert Alert)
        {
            await _AlertRepository.AddAlert(Alert);
            return "";
        }

        [HttpPut("{id}")]
        public async Task<bool> Put(string id, [FromBody] Alert Alert)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _AlertRepository.UpdateAlert(id, Alert);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _AlertRepository.RemoveAlert(id);
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            IEnumerable<Alert> alerts = _AlertRepository.Get();
            return View(alerts);
        }


    }
}