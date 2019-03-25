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

        public FileResult DownloadFile(string namefile, string station)
        {
            string[] files = namefile.Split(',');
            if (files.Length == 1){
                DirectoryInfo DI = new DirectoryInfo(Core.StationAudiosFolderPath(station));
                string fileAddress = DI.FullName + '/' + namefile;
                var net = new System.Net.WebClient();
                var data = net.DownloadData(fileAddress);
                var content = new System.IO.MemoryStream(data);

                return File(content, "audio/mp4", namefile);
            } else {
                var directory = Core.StationAudiosFolderPath(station);
                string archive = Path.Combine(Core.getBPVAudioDirectory() + "files", "audios.zip");
                var temp = Core.TemporaryFolderPath();

                if (System.IO.File.Exists(archive))
                {
                    System.IO.File.Delete(archive);
                }

                Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));

                foreach (var f in files)
                {
                    System.IO.File.Copy(Path.Combine(directory, f), Path.Combine(temp, f));
                }

                System.IO.Compression.ZipFile.CreateFromDirectory(temp, archive);

                return PhysicalFile(archive, "application/zip", "audios.zip");
            }
        }

        [HttpPut]
        public async Task<ActionResult> AddTag(int AudioId, string Tag)
        {
            await _audioRepository.AddTag(AudioId, Tag);
            return Content("Actualizado");
        }
    }
}