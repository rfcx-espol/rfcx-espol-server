using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Collections.Generic;
using System;

namespace WebApplication.Controllers
{        
    public class HistoricalDataController : Controller
    {
        private readonly IStationRepository _StationRepository;
        private readonly IDataRepository _DataRepository;
        public HistoricalDataController(
            IStationRepository  StationRepository,
            IDataRepository  DataRepository
        )
        {
                _StationRepository = StationRepository;
                _DataRepository = DataRepository;
        }        
        public IActionResult Index(){
            //retrieve stations
            var stations = _StationRepository.Get();
            ViewBag.message = "From server";
            return View(stations);
        }

        [Route("/ByHour")]
        public IActionResult ByHour(){
            //retrieve stations
            var stations = _StationRepository.Get();
            ViewBag.message = "From server";
            return View(stations);
        }

        [Route("/ByMonth")]
        public IActionResult ByMonth(){
            //retrieve stations
            var stations = _StationRepository.Get();
            ViewBag.message = "From server";
            return View(stations);
        }

        [Route("/ByDateStation")]
        public IActionResult ByDateStation(){            
            var sensorsTypeAndLocation = _DataRepository.sensorsTypeAndLocation();            
            List<string> l = new List<string>();
            foreach(var obj in sensorsTypeAndLocation){
                l.Add (obj["Type"].AsString + " " + obj["Location"].AsString);
                //Console.WriteLine(obj["Type"]);
            }
            ViewData["l"] = l;
            return View(ViewData);
        }
    }
}