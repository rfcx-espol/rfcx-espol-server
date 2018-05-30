using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;

namespace WebApplication {

    public class DeviceViewController : Controller {
        public IActionResult Index(string deviceName) {
            Console.WriteLine("dispositivo "+deviceName);
            ViewData["deviceName"] =deviceName;
            
            return View();
        }
    }
    
}