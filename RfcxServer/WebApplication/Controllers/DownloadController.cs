using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.FileProviders;
using System;
using Microsoft.IdentityModel.Protocols;
using System.Collections.Generic;
using System.Linq;
using Ionic.Zip;
using System.Text;
using System.Text.RegularExpressions;
using WebApplication.ViewModel;
using WebApplication.IRepository;
using WebApplication.Models;
using X.PagedList.Mvc.Core;
using X.PagedList;

namespace WebApplication
{

    public class DownloadController : Controller {

        private readonly IFileProvider _fileProvider;
        private readonly IStationRepository _stationRepository;
        private readonly IAudioRepository _audioRepository;

        public DownloadController(IFileProvider fileProvider, IStationRepository stationRepository, IAudioRepository audioRepository) {
            _fileProvider = fileProvider;
            _stationRepository = stationRepository;
            _audioRepository = audioRepository;
        }

        public static DateTime FromString(string offsetString)
        {
            DateTime offset;
            if (!DateTime.TryParse(offsetString, out offset))
            {
                offset = DateTime.Now;
            }

            return offset;
        }

        public string fileDate(string name)
        {
            int index = name.IndexOf('.');
            string first = name.Substring(0, index);
            var milliseconds = long.Parse(first);
            var date = DateTimeExtensions.DateTimeFromMilliseconds(milliseconds);
            return date.ToString();
        }

        public IActionResult Index()
        {
            var audioVM = new AudioViewModel()
                {
                    Stations = _stationRepository.Get()
                };
            return View(audioVM);
        }

        [HttpPost]
        public IActionResult List(AudioViewModel audioVM)
        {
            var pageNumber = (audioVM.Pnumber == 0) ? 1 : audioVM.Pnumber;
            var pageSize = 10;
            audioVM.Stations = _stationRepository.Get();
            var audios = _audioRepository.GetByStationAndDate(audioVM.StationId, audioVM.Start, audioVM.End).ToPagedList(pageNumber, pageSize);
            audioVM.Audios = audios;
            return View(audioVM);
        }

        public String getFile(String station, String audio){
            String file = Core.StationFolder(station)+"/"+audio;
            return file;
        }

        [HttpPost]
        public ActionResult DownloadFiles(string station, string lista) {

            string date = DateTime.Now.ToString("dd-MM-yyyy") + ".gz";
            station = Request.Form["station"];

            lista = Request.Form["lista"];
            string[] archivos_desc = lista.Split(",");
            Console.Write("STATION: "+ station);
            Console.Write("LISTA: "+lista);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            DirectoryInfo DI = new DirectoryInfo("files/" + station + "/audios");

            Ionic.Zip.ZipFile zip;

            var ifp = _fileProvider.GetDirectoryContents("/files/" + station + "/audios");
            
            using (zip = new Ionic.Zip.ZipFile())
            {
                foreach (var item in ifp)
                {
                    if (((item.Name.ToString().Substring(item.Name.ToString().IndexOf('.'), 4) == ".m4a") || 
                    (item.Name.ToString().Substring(item.Name.ToString().IndexOf('.'), 4) == ".3gp")) && 
                    item.Name.ToString().Length == 17 && archivos_desc.Any(s => s.Contains(item.Name.ToString())))
                    {
                        zip.AddFile(item.PhysicalPath, "");
                    }
                }
                zip.Save(DI.FullName + @"/" + date);
            }
            
            // DOWNLOADING FILE 
            string fileAddress = DI.FullName + @"/" + date;
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileAddress);
            var content = new System.IO.MemoryStream(data);

            System.IO.File.Delete("files/" + station + "/audios/"+ date);

            return File(content, "APPLICATION/octet-stream", date);
        }


        // Download a unique file by clicking the file in the showed list.
        public ActionResult DownloadUniqueFile(string namefile, string station)
        {
            DirectoryInfo DI = new DirectoryInfo("files/" + station + "/audios/");

            // DOWNLOADING FILE
            string fileAddress = DI.FullName + namefile;
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileAddress);
            var content = new System.IO.MemoryStream(data);

            return File(content, "APPLICATION/octet-stream", namefile);
        }
    }
}