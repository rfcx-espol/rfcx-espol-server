using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;

namespace WebApplication.Controllers
{        
    public class HistoricalDataController : Controller
    {
        private readonly IStationRepository _StationRepository;
        public HistoricalDataController(IStationRepository  StationRepository)
        {
                _StationRepository = StationRepository;
        }        
        public IActionResult Index(){
            //retrieve stations
            var stations = _StationRepository.Get();
            ViewBag.message = "From server";
            return View(stations);
        }
    }
}