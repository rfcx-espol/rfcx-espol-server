using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.FileProviders;
using System;

namespace WebApplication
{

    public class DownloadController : Controller {
        
        private readonly IFileProvider _fileProvider;
        public DownloadController(IFileProvider fileProvider) {
            _fileProvider = fileProvider;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, DateTime initialDate, DateTime finalDate) {
            
            //Comparing dates
            int result = DateTime.Compare(initialDate, finalDate);
            if (result > 0)
            {
                return Content("Dates not matching.");
            }

            //Verifyng Existing Files
            if (file == null || file.Length == 0)
                return Content("File not selected");


            //{ // Compression Test
            //    using (FileStream compressedStream = new FileStream(Core.GzipFolderPath + @"\" + file.FileName, FileMode.Open, FileAccess.ReadWrite))
            //    {

            //        using (FileStream outfolder = new FileStream(Core.GzipFolderPath + @"\" + file.FileName + ".gz",
            //                                                        FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //        {
            //            using (GZipStream gzip = new GZipStream(outfolder, CompressionMode.Compress))
            //            {
            //                compressedStream.CopyTo(gzip);
            //                //return File(gzip, "application/octet-stream", file.FileName);

            //                //var net = new System.Net.WebClient();
            //                //var data = net.DownloadData("https://www.google.com.ec/");
            //                ////var content = new System.IO.MemoryStream(gzip);
            //                //var contentType = "APPLICATION/octet-stream";
            //                //var fileName = file.FileName;
            //                //return File(gzip, contentType, fileName);
            //            }


            //        }
            //    }
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
            //string fileAddress = "http://localhost:59956/filesTest/" + file.FileName;
            //string fileAddress = "http://localhost:59956/filesTest/helloworld.html";
            //string fileAddress = "http://localhost:59956/prueba.jpeg.gz";
            string fileAddress = "http://localhost:59956/files/device1/gzip/" + file.FileName + ".gz";        
            //string fileAddress = "http://localhost:59956/Download/";
            //string myStringWebResource = fileAddress + file.FileName;

            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileAddress);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = file.FileName + ".gz";
            return File(content, contentType, fileName);


            //return Content("File received");
        }
    }
}