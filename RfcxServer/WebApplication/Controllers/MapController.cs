using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using WebApplication.Models;

namespace WebApplication {

    public class MapController : Controller {

        [Authorize(Policy = RolePolicy.PoliticaRoleTodos)]
        public IActionResult Index() {
            return View();
        }
    }
    
}