using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;

namespace WebApplication.Controllers
{        
    public class HistoricalDataController : Controller
    {
        private readonly IStationRepository _StationRepository;
        private readonly IDataRepository _DataRepository;
        
        private readonly ISensorRepository _SensorRepository;
        public HistoricalDataController(
            IStationRepository  StationRepository,
            IDataRepository  DataRepository,
            ISensorRepository SensorRepository
        )
        {
                _StationRepository = StationRepository;
                _DataRepository = DataRepository;
                _SensorRepository = SensorRepository;
        }        
        
        [Route("/ByDate")]
        public IActionResult ByDate(){
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
            var sensorsTypeAndLocation = _SensorRepository.sensorsTypeAndLocation();            
            List<string> l = new List<string>();
            foreach(var obj in sensorsTypeAndLocation){
                l.Add (obj["Type"].AsString + " " + obj["Location"].AsString);
                //Console.WriteLine(obj["Type"]);
            }
            ViewData["l"] = l;
            return View(ViewData);
        }

        //APIS with aggregated Data
        

        [HttpGet]
        [Route("api/test")]
        public Task<string> testA(
            [FromQuery] int StationId,
            [FromQuery] long StartTimestamp,
            [FromQuery] long EndTimestamp
        )
        {
            return this._testA(
                StationId,
                StartTimestamp, 
                EndTimestamp
            );
        }

        //this api is for testing remove it once accomplishes its purpose
        private async Task<string> _testA(
            int StationId,
            long StartTimestamp,
            long EndTimestamp
        )
        {

            var sensors = await _SensorRepository.GetByStation(StationId);                
            var arr = new JArray();
            foreach(var s in sensors){
                var SensorId = s.Id;
                var SensorType = s.Type;                 
                var SensorLocation = s.Location;                           
                //Console.WriteLine(s.Id);

                dynamic obj = new JObject();
                IEnumerable<BsonDocument> data_ = await _DataRepository.testA(
                    StationId,
                    SensorId,
                    StartTimestamp, 
                    EndTimestamp
                ); 
                
                obj.SensorId = SensorId;                
                obj.SensorType = SensorType;
                obj.SensorLocation = SensorLocation;
                obj.aggregates = new JArray();
                foreach(var x in data_){
                    //extract values from the result of mongo aggregation
                    var date    = x["date"].ToUniversalTime();
                    var average = x["average"].ToDouble();

                    dynamic aggregate = new JObject();
                    aggregate.average = average;
                    aggregate.date    = date;

                    obj.aggregates.Add(aggregate);
                }
                arr.Add(obj);
            }
            return arr.ToString(); 
        }

        [HttpGet]
        [Route("api/testB")]
        public Task<string> testB(
            [FromQuery] int StationId,
            [FromQuery] long StartTimestamp,
            [FromQuery] long EndTimestamp
        )
        {
            return this._testB(
                StationId,
                StartTimestamp, 
                EndTimestamp
            );
        }

        //this api is for testing remove it once accomplishes its purpose
        private async Task<string> _testB(
            int StationId,
            long StartTimestamp,
            long EndTimestamp
        )
        {

            var sensors = await _SensorRepository.GetByStation(StationId);                
            var arr = new JArray();
            foreach(var s in sensors){
                var SensorId = s.Id;                
                var SensorType = s.Type;                 
                var SensorLocation = s.Location;                    
                //Console.WriteLine(s.Id);

                dynamic obj = new JObject();
                IEnumerable<BsonDocument> data_ = await _DataRepository.testB(
                    StationId,
                    SensorId,
                    StartTimestamp, 
                    EndTimestamp
                ); 
                
                obj.SensorId = SensorId;                                  
                obj.SensorType = SensorType;
                obj.SensorLocation = SensorLocation;
                obj.aggregates = new JArray();
                foreach(var x in data_){
                    //extract values from the result of mongo aggregation
                    var hour    = x["hour"].ToInt32();
                    var average = x["average"].ToDouble();

                    dynamic aggregate = new JObject();
                    aggregate.average = average;
                    aggregate.hour    = hour;

                    obj.aggregates.Add(aggregate);
                }
                arr.Add(obj);
            }
            return arr.ToString(); 
        }

        [HttpGet]
        [Route("api/testC")]
        public Task<string> testC(
            [FromQuery] int StationId,
            [FromQuery] long StartTimestamp,
            [FromQuery] long EndTimestamp
        )
        {
            return this._testC(
                StationId,
                StartTimestamp, 
                EndTimestamp
            );
        }
        private async Task<string> _testC(
            int StationId,
            long StartTimestamp,
            long EndTimestamp
        )
        {

            var sensors = await _SensorRepository.GetByStation(StationId);                
            var arr = new JArray();
            foreach(var s in sensors){
                var SensorId = s.Id;                                   
                var SensorType = s.Type;                 
                var SensorLocation = s.Location;                    
                //Console.WriteLine(s.Id);

                dynamic obj = new JObject();
                IEnumerable<BsonDocument> data_ = await _DataRepository.testC(
                    StationId,
                    SensorId,
                    StartTimestamp, 
                    EndTimestamp
                ); 
                
                obj.SensorId = SensorId;                                                   
                obj.SensorType = SensorType;
                obj.SensorLocation = SensorLocation;    
                obj.aggregates = new JArray();
                foreach(var x in data_){
                    //extract values from the result of mongo aggregation
                    var month   = x["month"].ToInt32();
                    var average = x["average"].ToDouble();

                    dynamic aggregate = new JObject();
                    aggregate.average = average;
                    aggregate.month    = month;

                    obj.aggregates.Add(aggregate);
                }
                arr.Add(obj);
            }
            return arr.ToString(); 
        }

        [HttpGet]
        [Route("api/testD")]
        public Task<string> testD(
            [FromQuery] string SensorType,
            [FromQuery] string SensorLocation,
            [FromQuery] long StartTimestamp,
            [FromQuery] long EndTimestamp
        )
        {
            return this._testD(             
                SensorType,
                SensorLocation,
                StartTimestamp, 
                EndTimestamp
            );
        }
        private async Task<string> _testD(
            string SensorType,
            string SensorLocation,
            long StartTimestamp,
            long EndTimestamp
        )
        {
            
            var stations = _StationRepository.Get();
            var arr = new JArray();
            foreach(var s in stations){
                var StationId = s.Id;                              
                var StationName = s.Name;   
                //Console.WriteLine(s.Id);

                dynamic obj = new JObject();
                IEnumerable<BsonDocument> data_ = await _DataRepository.testD(
                    StationId,
                    SensorType,
                    SensorLocation,
                    StartTimestamp, 
                    EndTimestamp
                ); 
                
                obj.StationId = StationId;
                obj.StationName = StationName;
                obj.aggregates = new JArray();
                foreach(var x in data_){
                    //extract values from the result of mongo aggregation
                    var date   = x["date"].ToUniversalTime();
                    var average = x["average"].ToDouble();

                    dynamic aggregate = new JObject();
                    aggregate.average = average;
                    aggregate.date    = date;

                    obj.aggregates.Add(aggregate);
                }
                arr.Add(obj);
            }
            return arr.ToString(); 
        }
    }
}