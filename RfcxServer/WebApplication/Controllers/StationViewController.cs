using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;
namespace WebApplication {

    public class StationViewController : Controller {
        public IActionResult Index(string stationName, int stationId) {
            Console.WriteLine(stationName);
            ViewData["stationName"] =stationName;
            ViewData["stationId"]= stationId;           
            return View();
        }
    }

    
}