using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class HistoricalDataController : Controller
    {        
        public IActionResult Index(){
            ViewBag.message = "From server";
            return View();
        }
    }
}