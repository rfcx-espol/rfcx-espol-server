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

namespace WebApplication
{

    public class IndexModel
    {
        public IDirectoryContents Devices { get; set; }
        public IDirectoryContents Files { get; set; }
        public String selected { get; set; }
        public String start { get; set; }
        public String end { get; set; }
        public DateTimeOffset start_d { get; set; }
        public DateTimeOffset end_d { get; set; }
    }

    public class DownloadController : Controller {

        private readonly IFileProvider _fileProvider;

        public DownloadController(IFileProvider fileProvider) {
            _fileProvider = fileProvider;
        }

        public static DateTimeOffset FromString(string offsetString)
        {

            DateTimeOffset offset;
            if (!DateTimeOffset.TryParse(offsetString, out offset))
            {
                offset = DateTimeOffset.Now;
            }

            return offset;
        }

        //public IActionResult Index(string device)
        public IActionResult Index(string dd1)
        {
            string device = "device1";
            string selected = "device1";
            string start = null;
            string end = null;
            DateTimeOffset start_d, end_d;
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
            var contents = _fileProvider.GetDirectoryContents("/files/" + device + "/gzip");
            IndexModel content = new IndexModel();
            content.Files = contents;
            content.Devices = _fileProvider.GetDirectoryContents("/files/");
            content.selected = selected;
            content.start = start;
            content.end = end;
            if(start_d != null && end_d!= null) {
                content.start_d = start_d;
                content.end_d = end_d;

            }


            return View(content);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadFiles(string device, DateTime initialDate, DateTime finalDate) {

            ////Comparing dates
            //int result = DateTime.Compare(initialDate, finalDate);
            //if (result > 0)
            //{
            //    return Content("Dates not matching.");
            //}



            string date = DateTime.Now.ToString("yyyyMMdd") + ".gz";
            device = "device1";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            DirectoryInfo DI = new DirectoryInfo("files/" + device + "/gzip");
            Ionic.Zip.ZipFile zip;
            using (zip = new Ionic.Zip.ZipFile())
            {
                // Add the file to the Zip archive's root folder.
                // zip.AddFile(item.Name, item.DirectoryName);
                // Save the Zip file.
                zip.AddDirectory( DI.FullName);
                zip.ParallelDeflateThreshold = -1;
                zip.Save(DI.FullName + @"/" + date);
            }

            // DOWNLOADING FILE (.tg)
            string fileAddress = DI.FullName + @"/" + date;
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileAddress);
            var content = new System.IO.MemoryStream(data);

            System.IO.File.Delete("files/" + device + "/gzip/"+ date);

            return File(content, "APPLICATION/octet-stream", date);
        }


        // Download a unique file by clicking the file in the showed list.
        public ActionResult DownloadUniqueFile(string namefile, string device)
        {
            device = "device1";

            DirectoryInfo DI = new DirectoryInfo("files/" + device + "/gzip/");

            //var milliseconds = long.Parse("1508869459001");
            //var date = DateTimeExtensions.DateTimeFromMilliseconds(milliseconds);
            //var localDate = date.ToLocalTime();

            // DOWNLOADING FILE
            string fileAddress = DI.FullName + namefile;
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileAddress);
            var content = new System.IO.MemoryStream(data);

            return File(content, "APPLICATION/octet-stream", namefile);
        }

    }
}