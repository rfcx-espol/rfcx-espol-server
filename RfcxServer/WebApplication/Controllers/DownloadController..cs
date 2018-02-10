using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.FileProviders;
using System;
using Microsoft.IdentityModel.Protocols;
using System.Collections.Generic;

namespace WebApplication
{

    public class DownloadController : Controller {
        
        private readonly IFileProvider _fileProvider;

        public DownloadController(IFileProvider fileProvider) {
            _fileProvider = fileProvider;
        }
        
        public IActionResult Index()
        {
            //var contents = _fileProvider.GetDirectoryContents("http://localhost:59956/files/device1/gzip/");
            var contents = _fileProvider.GetDirectoryContents("/files/device1/gzip");
            return View(contents);
            //return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, DateTime initialDate, DateTime finalDate) {
            
            ////Comparing dates
            //int result = DateTime.Compare(initialDate, finalDate);
            //if (result > 0)
            //{
            //    return Content("Dates not matching.");
            //}
            
            ////Verifyng Existing Files
            //if (file == null || file.Length == 0)
            //    return Content("File not selected");


            
            //DirectoryInfo DI = new DirectoryInfo("files/device1/gzip");
            //foreach (var item in DI.GetFiles())
            //{
            //    FileStream compressedStream = new FileStream(Core.GzipFolderPath + @"\" + item.Name, FileMode.Open, FileAccess.ReadWrite);
            //    FileStream outfolder = new FileStream(Core.GzipFolderPath + @"\" + item.Name + ".gz", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //    GZipStream gzip = new GZipStream(outfolder, CompressionMode.Compress);
            //    compressedStream.CopyTo(gzip);

            //    gzip.Close();
            //    outfolder.Close();
            //    compressedStream.Close();
            //}

            // COMPRESSING FILE
            FileStream compressedStream = new FileStream(Core.GzipFolderPath + @"\" + file.FileName, FileMode.Open, FileAccess.ReadWrite);
            FileStream outfolder = new FileStream(Core.GzipFolderPath + @"\" + file.FileName + ".gz", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            GZipStream gzip = new GZipStream(outfolder, CompressionMode.Compress);
            compressedStream.CopyTo(gzip);

            gzip.Close();
            outfolder.Close();
            compressedStream.Close();


            // DOWNLOADING FILE (.tg)
            string fileAddress = "http://localhost:59956/files/device1/gzip/" + file.FileName + ".gz";
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileAddress);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = file.FileName + ".gz";
            //var fileName = "prueba.gz";

            System.IO.File.Delete("files/device1/gzip/" + file.FileName + ".gz");

            return File(content, contentType, fileName);
            

        }
    }
}