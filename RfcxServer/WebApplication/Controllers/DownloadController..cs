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

            string date = DateTime.Now.ToString("yyyyMMdd") + ".gz";
            device = Request.Form["device"];
            string start = Request.Form["start"];
            string end = Request.Form["end"];
            DateTimeOffset start_d, end_d;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            DirectoryInfo DI = new DirectoryInfo("files/" + device + "/gzip");
            Ionic.Zip.ZipFile zip;

            var ifp = _fileProvider.GetDirectoryContents("/files/" + device + "/gzip");
            
            using (zip = new Ionic.Zip.ZipFile())
            {
                foreach (var item in ifp)
                {
                    
                    if (item.Name.ToString().Substring(item.Name.ToString().IndexOf('.'), 5) == ".flac" && item.Name.ToString().Length == 18)
                    {
                        zip.AddFile(item.PhysicalPath, "");
                    }
                }
                zip.Save(DI.FullName + @"/" + date);
            }
            
                //using (zip = new Ionic.Zip.ZipFile())
                //{
                //    // Add the file to the Zip archive's root folder.
                //    // zip.AddFile(item.Name, item.DirectoryName);
                //    // Save the Zip file.

                //    zip.AddDirectory(DI.FullName);
                //    zip.ParallelDeflateThreshold = -1;
                //    zip.Save(DI.FullName + @"/" + date);
                //}

                // DOWNLOADING FILE 
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
            device = Request.Form["device"];
            //device = "device1";

            DirectoryInfo DI = new DirectoryInfo("files/" + device + "/gzip/");

            // DOWNLOADING FILE
            string fileAddress = DI.FullName + namefile;
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileAddress);
            var content = new System.IO.MemoryStream(data);

            return File(content, "APPLICATION/octet-stream", namefile);
        }


    }
}
