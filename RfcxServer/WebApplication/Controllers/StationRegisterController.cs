using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplication {

    public class StationRegisterController : Controller {
        public IActionResult Index() {
            var client = new HttpClient();
            var response = client.GetAsync("http://127.0.0.1:5000/api/Station").Result;
            var stations = response.Content.ReadAsStringAsync().Result;
            ViewData["stations"] = JArray.Parse(stations.ToString());
            return View();
        }
    }
    
}
