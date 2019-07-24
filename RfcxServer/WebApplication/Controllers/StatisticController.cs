using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class StatisticController : Controller
    {        
        public IActionResult Index(){
            ViewBag.message = "From server";
            return View();
        }
    }
}