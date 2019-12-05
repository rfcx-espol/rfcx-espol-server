using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using WebApplication.IRepository;
using WebApplication.Models;
using WebApplication.ViewModel;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication.Controllers
{
    public class IncidentController : Controller
    {
        private readonly IIncidentRepository _IncidentRepository;
        public IncidentController(IIncidentRepository IncidentRepository)
        {
            _IncidentRepository = IncidentRepository;
        }

        [HttpGet("api/[controller]/list")]
        public async Task<string> GetAllIncidents()
        {
            var Alerts = await _IncidentRepository.GetAllIncidents();
            return JsonConvert.SerializeObject(Alerts);
        }


        [HttpGet("api/[controller]/{id}")]
        public async Task<string> Get(string id)
        {
            var Incident = await _IncidentRepository.GetIncident(id) ?? new Incident();
            return JsonConvert.SerializeObject(Incident);
        }

        [HttpPost("api/[controller]")]
        public async Task Post([FromBody] Incident incident)
        {
            // var fromAddress = new MailAddress("mail@gmail.com", "Bosque Protector");
            // var toAddress = new MailAddress("mail@hotmail.com", "");
            // const string fromPassword = "";
            // const string subject = "Incidente en tal estacion";
            // const string body = "Se ha generado un incidente a tal hora en tal estacion, por favor visitar el link";

            // var smtp = new SmtpClient
            // {
            //     Host = "smtp.gmail.com",
            //     Port = 587,
            //     EnableSsl = true,
            //     DeliveryMethod = SmtpDeliveryMethod.Network,
            //     UseDefaultCredentials = false,
            //     Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            // };
            // using (var message = new MailMessage(fromAddress, toAddress)
            // {
            //     Subject = subject,
            //     Body = body
            // })
            // {
            //     smtp.Send(message);
            // }
            await _IncidentRepository.AddIncident(incident);
        }

        [HttpPut("api/[controller]/{id}")]
        public async Task<bool> Put(string id, [FromBody] Incident incident)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _IncidentRepository.UpdateIncident(id, incident);
        }

        [HttpDelete("api/[controller]/{id}")]
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return await _IncidentRepository.RemoveIncident(id);
        }

        [HttpPatch("api/[controller]/{id}/status")]
        public async Task<bool> UpdateStatus(string id, [FromBody] Boolean status)
        {
            bool result = await _IncidentRepository.UpdateIncidentStatus(id, status);
            if(result == true)
                TempData["editResult"] = 1;
            else
                TempData["editResult"] = -1;
            return result;
        }

        [Authorize(Policy = RolePolicy.PoliticaRoleTodos)]
        [HttpGet("[controller]/index")]
        public IActionResult Index(IncidentViewModel iVM)
        {
            var pageNumber = (iVM.Pnumber == 0) ? 1 : iVM.Pnumber;
            var pageSize = 10;
            var incidents = _IncidentRepository.GetAll().ToPagedList(pageNumber, pageSize);
            iVM.Incidents = incidents;
            if(TempData["editResult"] == null)
                TempData["editResult"] = 0;
            return View(iVM);
        }

        /* TRIED BUT FAILED TO FILTER INCIDENTS BY DATE
        [HttpPost()]
        public IActionResult List(IncidentViewModel iVM)
        {
            var pageNumber = (iVM.Pnumber == 0) ? 1 : iVM.Pnumber;
            var pageSize = 10;
            var incidents = _IncidentRepository.GetByDate(iVM.Start, iVM.End).ToPagedList(pageNumber, pageSize);
            iVM.Incidents = incidents;
            if(TempData["editResult"] == null)
                TempData["editResult"] = 0;
            return View(iVM);
        }
        */
    }
}