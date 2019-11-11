using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize(Policy = RolePolicy.PoliticaRoleTodos)]
    public class HomeController : Controller
    {
        public IActionResult Index(){
            return View();
        }
    }
}