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

namespace WebApplication
{

    public class IndexModel
    {
        public IDirectoryContents Devices { get; set; }
        public IDirectoryContents Files { get; set; }
        public String selected { get; set; }
        public String start { get; set; }
        public String end { get; set; }
        public DateTime start_d { get; set; }
        public DateTime end_d { get; set; }

        public List<string> deviceFolders { get; set; }
        public IFileInfo[] filesSorted { get; set; }
    }

    public class DownloadController : Controller {

        private readonly IFileProvider _fileProvider;

        public DownloadController(IFileProvider fileProvider) {
            _fileProvider = fileProvider;
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

        public IActionResult Index(string dd1)
        {
            string device = "";
            string selected = "";
            string start = null;
            string end = null;
            DateTime start_d, end_d;
            if (Request.Method == "POST")
            {
                device = Request.Form["ddl"];
                selected = device;
                start = Request.Form["start"];
                end = Request.Form["end"];
                if(start.Length >0 && end.Length > 0)
                {
                    start_d = FromString(start);
                    end_d = FromString(end);
                }
            }

            start_d = FromString(start);
            end_d = FromString(end);

            IndexModel content = new IndexModel();
            content.Files = _fileProvider.GetDirectoryContents("/files/" + device + "/audios");
            content.Devices = _fileProvider.GetDirectoryContents("/files/");

            IFileInfo[] files = _fileProvider.GetDirectoryContents("/files/" + device + "/audios").OrderBy(p => p.LastModified).ToArray();

            content.filesSorted = files;

            string pattern = @"^(device)[(0-9)]";
            List<string> df = new List<string>();
            foreach (var item in content.Devices)
            {
                if (Regex.IsMatch(item.Name.ToString(), pattern, RegexOptions.IgnoreCase))
                {
                    df.Add(item.Name.ToString());
                }
            }

            content.deviceFolders = df;

            content.selected = selected;
            content.start = start;
            content.end = end;
            if (start_d != null && end_d != null)
            {
                content.start_d = start_d;
                content.end_d = end_d;
            }

           
            return View(content);
        }
        public String getFile(String device, String audio){
            String file = Core.DeviceFolderPath(device)+"/"+audio;
            return file;
        }

        [HttpPost]
        public async Task<IActionResult> DownloadFiles(string device, DateTime initialDate, DateTime finalDate) {

            string date = DateTime.Now.ToString("yyyyMMdd") + ".gz";
            device = Request.Form["device"];
            string start = Request.Form["start"];
            string end = Request.Form["end"];
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            DirectoryInfo DI = new DirectoryInfo("files/" + device + "/audios");
            Ionic.Zip.ZipFile zip;

            var ifp = _fileProvider.GetDirectoryContents("/files/" + device + "/audios");
            
            using (zip = new Ionic.Zip.ZipFile())
            {
                foreach (var item in ifp)
                {
                    if (((item.Name.ToString().Substring(item.Name.ToString().IndexOf('.'), 4) == ".m4a") || (item.Name.ToString().Substring(item.Name.ToString().IndexOf('.'), 4) == ".3gp")) && item.Name.ToString().Length == 17)
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

            System.IO.File.Delete("files/" + device + "/audios/"+ date);

            return File(content, "APPLICATION/octet-stream", date);
        }


        // Download a unique file by clicking the file in the showed list.
        public ActionResult DownloadUniqueFile(string namefile, string device)
        {
            DirectoryInfo DI = new DirectoryInfo("files/" + device + "/audios/");

            // DOWNLOADING FILE
            string fileAddress = DI.FullName + namefile;
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileAddress);
            var content = new System.IO.MemoryStream(data);

            return File(content, "APPLICATION/octet-stream", namefile);
        }


    }
}
