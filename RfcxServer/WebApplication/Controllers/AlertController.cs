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

        [HttpGet("{alertId}/condition/list")]
        public async Task<string> GetConditions(string alertId)
        {
            var Alert = await _AlertRepository.GetAlert(alertId) ?? new Alert();
            return JsonConvert.SerializeObject(Alert.Conditions);

        }
        [HttpGet("{alertId}/condition/{conditionId}")]
        public string GetCondition(int alertId, int conditionId)
        {
            Condition condition = _AlertRepository.getConditionObject(alertId, conditionId);

            return JsonConvert.SerializeObject(condition);

        }

        [HttpPatch("{alertId}/condition")]
        public async Task<bool> AddCondition(string alertId, [FromBody] Condition condition)
        {

            var Alert = await _AlertRepository.GetAlert(alertId) ?? new Alert();
            Alert.Conditions.Add(condition);
            return await _AlertRepository.UpdateAlert(alertId, Alert);
        }


        // [HttpPatch("{alertId}/condition/{conditionId}")]
        // public async Task<bool> EditCondition(int alertId, int conditionId, [FromBody] Condition condition)
        // {

        //     var Alert = await _AlertRepository.GetAlert(alertId.ToString()) ?? new Alert();
        //     Condition c = _AlertRepository.getConditionObject(alertId, conditionId);
        //     Alert.Conditions
        // }

        [HttpDelete("{alertId}/condition/{conditionId}")]
        public async Task<bool> DeleteCondition(string alertId, int conditionId)
        {
            if (string.IsNullOrEmpty(alertId))
            {
                return false;
            }
            var Alert = await _AlertRepository.GetAlert(alertId) ?? new Alert();
            int index = Alert.Conditions.FindIndex(x => x.Id == conditionId);
            Alert.Conditions.RemoveAt(index);

            return await _AlertRepository.UpdateAlert(alertId, Alert);

        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            IEnumerable<Alert> alerts = _AlertRepository.Get();
            return View(alerts);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id) {
            IEnumerable<Alert> alerts = _AlertRepository.Get();
            return View();
        }
    }
}