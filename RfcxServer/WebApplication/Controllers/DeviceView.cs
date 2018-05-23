using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace WebApplication {

    public class DeviceViewController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
    
}