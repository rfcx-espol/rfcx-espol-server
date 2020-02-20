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
using System.Linq;

namespace WebApplication.Controllers
{
    public class IncidentController : Controller
    {
        private readonly IIncidentRepository _IncidentRepository;
        private readonly IAlertRepository _AlertRepository;
        public IncidentController(IIncidentRepository IncidentRepository, IAlertRepository AlertRepository)
        {
            _IncidentRepository = IncidentRepository;
            _AlertRepository = AlertRepository;
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

            
            var cred = System.IO.File.ReadLines("/var/alert-daemon/cred.txt");
            var list = cred.ToList();
            
            Alert alert = _AlertRepository.getAlertObject(incident.RaisedAlertId);
            var fromAdress = new MailAddress(list[0], "Bosque Protector");
            string fromPassword = list[1];
            List<string> mailto = alert.Mailto;
            
            const string subject = "Incidente en el bosque";
            string htmlString = @"<html>
                    <body>
                    <p>Se ha detectado un incidente en el bosque:</p>
                    <p>" + alert.Message +
                    "<p>Alerta: " + incident.RaisedAlertName + @"</p>
                    <p>Las siguientes condiciones se cumplieron: " + incident.RaisedCondition + @"</p>
                    <p>Favor visitar <a href='http://200.126.14.250/alert/index'>Bosque Protector</a>
                    </body>
                    </html>";  


            var smtp = new SmtpClient
            {
                
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromAdress.Address, fromPassword)
            };

            var msg = new MailMessage();
            msg.From = fromAdress;
            foreach(string mail in mailto){
                msg.To.Add(mail);
            }
            
            msg.Subject = subject;
            msg.Body = htmlString;
            msg.IsBodyHtml= true;
            try{
                smtp.Send(msg);
            }catch(Exception ex){
                throw ex;
            }
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