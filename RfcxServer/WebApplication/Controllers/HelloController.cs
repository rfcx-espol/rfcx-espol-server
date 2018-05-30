using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace WebApplication {

    public class HelloController : Controller {
        public IActionResult Index() {
            return View();
        }

        public string Moon() {
            return "Hello Moon";
        }
    }
    
}