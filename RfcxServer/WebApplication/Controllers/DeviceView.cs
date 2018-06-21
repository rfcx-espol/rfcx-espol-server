using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;

namespace WebApplication {

    public class DeviceViewController : Controller {
        public IActionResult Index(string deviceName, int deviceId, int sensorId) {
            Console.WriteLine("id sensor "+sensorId);
            ViewData["deviceName"] =deviceName;
            ViewData["deviceId"]= deviceId;
            ViewData["sensorId"]= sensorId;
            
            return View();
        }
    }
    
}