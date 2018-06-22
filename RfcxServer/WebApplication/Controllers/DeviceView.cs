using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;

namespace WebApplication {

    public class DeviceViewController : Controller {
        public IActionResult Index(string deviceName, int deviceId) {
            ViewData["deviceName"] =deviceName;
            ViewData["deviceId"]= deviceId;           
            return View();
        }
    }
    
}