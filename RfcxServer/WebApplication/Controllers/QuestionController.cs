using Microsoft.AspNetCore.Mvc;
using WebApplication.IRepository;
using System.Threading.Tasks;
using WebApplication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Net;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Encodings.Web;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using WebApplication.Controllers;
using WebApplication.Repository;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Web;
using System.Drawing;
using System.Net.Http;
using Microsoft.IdentityModel.Protocols;
using Ionic.Zip;
using System.Text.RegularExpressions;

namespace WebApplication
{

    [Route("api/bpv/[controller]")]
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository _QuestionRepository;
        private readonly ISpecieRepository _SpecieRepository;
        private readonly IStationRepository _StationRepository;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public QuestionController(IQuestionRepository QuestionRepository, ISpecieRepository SpecieRepository,
                                    IStationRepository StationRepository)
        {
            _QuestionRepository = QuestionRepository;
            _SpecieRepository = SpecieRepository;
            _StationRepository = StationRepository;
        }

        [HttpGet("index")]
        public IActionResult Index() {
            if(TempData["creacion"] == null)
                TempData["creacion"] = 0;
            if(TempData["eliminacion"] == null)
                TempData["eliminacion"] = 0;
            if(TempData["edicion"] == null)
                TempData["edicion"] = 0;
            IDictionary<Question, Specie> diccionario = new Dictionary<Question, Specie>();
            List<Question> preguntas = _QuestionRepository.Get();
            foreach(Question pregunta in preguntas) {
                Specie especie = _SpecieRepository.Get(pregunta.SpecieId);
                diccionario.Add(pregunta, especie);
            }
            ViewBag.diccionario = diccionario;
            return View();
        }

        [HttpGet("create")]
        public IActionResult Create() {
            ViewBag.especies = _SpecieRepository.Get();
            List<Station> estaciones = _StationRepository.Get();
            ViewBag.estaciones = estaciones;
            return View();
        }

        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id) {
            ViewBag.especies = _SpecieRepository.Get();
            ViewBag.pregunta = _QuestionRepository.Get(id);
            List<Station> estaciones = _StationRepository.Get();
            ViewBag.estaciones = estaciones;
            return View();
        }

        [HttpGet()]
        public string Get()
        {
            return this.GetQuestion();
        }

        private string GetQuestion()
        {
            var Question = _QuestionRepository.Get();
            return JsonConvert.SerializeObject(Question);
        }

        [HttpGet("{id:int}")]
        public string Get(int id)
        {
            return this.GetQuestionByIdInt(id);
        }

        private string GetQuestionByIdInt(int id)
        {
            var Question = _QuestionRepository.Get(id) ?? new Question();
            return JsonConvert.SerializeObject(Question);
        }

        [HttpPost]
        public IActionResult Post()
        {
            Question question = new Question();
            List<Station> lista_estaciones = new List<Station>();
            question.SpecieId = Int32.Parse(Request.Form["especie"]);
            question.Text = Request.Form["pregunta"];
            question.Options = new List<string>();
            question.Options.Add(Request.Form["opcion_1"]);
            question.Options.Add(Request.Form["opcion_2"]);
            question.Options.Add(Request.Form["opcion_3"]);
            question.Options.Add(Request.Form["opcion_4"]);
            question.Answer = Int32.Parse(Request.Form["respuesta"]);
            question.Feedback = Request.Form["retroalimentacion"];
            question.Category = Request.Form["categoria"];
            string estaciones = Request.Form["estaciones"];
            string[] ids = estaciones.Split(",");
            foreach(string id in ids) {
                Station estacion = _StationRepository.Get(Int32.Parse(id));
                lista_estaciones.Add(estacion);
            }
            question.Stations = lista_estaciones;
            bool result = _QuestionRepository.Add(question);
            if(result == true)
                TempData["creacion"] = 1;
            else
                TempData["creacion"] = -1;
            return Redirect("/api/bpv/question/index/");
        }

        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            bool valor = _QuestionRepository.Remove(id);
            if(valor == true) {
                TempData["eliminacion"] = 1;
            } else {
                TempData["eliminacion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/SpecieId")]
        public bool PatchSpecieId(int id, [FromBody] Arrays json)
        {
            bool valor = _QuestionRepository.UpdateSpecieId(id, json.SpecieId);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Text")]
        public bool PatchText(int id, [FromBody] Arrays json)
        {
            bool valor = _QuestionRepository.UpdateText(id, json.Text);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Option")]
        public bool PatchOption(int id, [FromBody] Arrays json)
        {
            bool valor = _QuestionRepository.UpdateOption(id, json.Index, json.Option);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Answer")]
        public bool PatchAnswer(int id, [FromBody] Arrays json)
        {
            bool valor = _QuestionRepository.UpdateAnswer(id, json.Answer);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Feedback")]
        public bool PatchFeedback(int id, [FromBody] Arrays json)
        {
            bool valor = _QuestionRepository.UpdateFeedback(id, json.Feedback);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id}/Category")]
        public bool PatchCategory(int id, [FromBody] Arrays json)
        {
            bool valor = _QuestionRepository.UpdateCategory(id, json.Category);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

        [HttpPatch("{id_pregunta}/Stations")]
        public bool PatchStations(int id_pregunta, [FromBody] Arrays json)
        {
            List<Station> stations = new List<Station>();
            string estaciones = json.Stations;
            string[] ids = estaciones.Split(",");
            foreach(string id in ids) {
                Station estacion = _StationRepository.Get(Int32.Parse(id));
                stations.Add(estacion);
            }
            bool valor = _QuestionRepository.UpdateStations(id_pregunta, stations);
            if(valor == true) {
                TempData["edicion"] = 1;
            } else {
                TempData["edicion"] = -1;
            }
            return true;
        }

    }

}