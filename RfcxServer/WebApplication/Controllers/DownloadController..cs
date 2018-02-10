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
        
    }

    public class DownloadController : Controller {
        
        private readonly IFileProvider _fileProvider;

        public DownloadController(IFileProvider fileProvider) {
            _fileProvider = fileProvider;
        }

        public IActionResult Index()
        {
            string device = "device1";
            //if (Request.Method == "POST")
            //{
            //    device = Request.HttpContext["device1"];
            //}
            var contents = _fileProvider.GetDirectoryContents("/files/" + device + "/gzip");
            IndexModel content = new IndexModel();
            content.Files = contents;
            content.Devices = _fileProvider.GetDirectoryContents("/files/");


            return View(content);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadFiles(String device, DateTime initialDate, DateTime finalDate) {


            ////Comparing dates
            //int result = DateTime.Compare(initialDate, finalDate);
            //if (result > 0)
            //{
            //    return Content("Dates not matching.");
            //}

            ////Verifyng Existing Files
            //if (file == null || file.Length == 0)
            //    return Content("File not selected");

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
                zip.Save(DI.FullName+@"/comprimido.gz");
            }
            
            // DOWNLOADING FILE (.tg)
            string fileAddress = DI.FullName+@"/comprimido.gz";
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileAddress);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = "comprimido.gz";
            
            System.IO.File.Delete("files/" + device + "/gzip/comprimido.gz");

            return File(content, contentType, fileName);
            
        }
    }
}