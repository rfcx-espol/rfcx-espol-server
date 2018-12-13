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
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public QuestionController(IQuestionRepository QuestionRepository, ISpecieRepository SpecieRepository)
        {
            _QuestionRepository = QuestionRepository;
            _SpecieRepository = SpecieRepository;
        }

        [HttpGet("create")]
        public IActionResult Index() {
            ViewBag.especies = _SpecieRepository.Get();
            if(TempData["exito"] == null)
                TempData["exito"] = 0;
            return View();
        }

        [HttpGet()]
        public Task<string> Get()
        {
            return this.GetQuestion();
        }

        private async Task<string> GetQuestion()
        {
            var Question = await _QuestionRepository.Get();
            return JsonConvert.SerializeObject(Question);
        }

        [HttpGet("{id:int}")]
        public Task<string> Get(int id)
        {
            return this.GetQuestionByIdInt(id);
        }

        private async Task<string> GetQuestionByIdInt(int id)
        {
            var Question = await _QuestionRepository.Get(id) ?? new Question();
            return JsonConvert.SerializeObject(Question);
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            Question question = new Question();
            question.SpecieId = Int32.Parse(Request.Form["especie"]);
            question.Text = Request.Form["pregunta"];
            question.Options = new List<string>();
            question.Options.Add(Request.Form["opcion_1"]);
            question.Options.Add(Request.Form["opcion_2"]);
            question.Options.Add(Request.Form["opcion_3"]);
            question.Options.Add(Request.Form["opcion_4"]);
            question.Answer = Int32.Parse(Request.Form["respuesta"]);
            question.Feedback = Request.Form["retroalimentacion"];
            Task result = _QuestionRepository.Add(question);
            TempData["exito"] = 1;
            return Redirect("/api/bpv/question/create/");
        }

    }

}