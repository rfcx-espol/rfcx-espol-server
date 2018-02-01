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
            

            { // Compression Test
                using (FileStream compressedStream = new FileStream(Core.GzipFolderPath + @"\" + file.FileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    using (FileStream outfolder = new FileStream(Core.GzipFolderPath + @"\"+ file.FileName + ".gz", 
                                                                    FileMode.Open, FileAccess.ReadWrite))
                    {
                        using (GZipStream gzip = new GZipStream(outfolder, CompressionMode.Compress)) {
                            compressedStream.CopyTo(gzip);
                        }
                    }
                }
            }
            return Content("File received");
        }
    }
}