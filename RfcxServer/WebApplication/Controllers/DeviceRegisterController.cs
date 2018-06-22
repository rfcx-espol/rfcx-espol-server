using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplication {

    public class DeviceRegisterController : Controller {
        public IActionResult Index() {
            var client = new HttpClient();
            var response = client.GetAsync("http://127.0.0.1:5000/api/Device").Result;
            var devices = response.Content.ReadAsStringAsync().Result;
            //ViewData["devices"] = JSON.parse(devices);
            ViewData["devices"] = JArray.Parse(devices.ToString());
            //ViewData["devices"] = JsonConvert.SerializeObject(devices);
            return View();
        }
    }
    
}
