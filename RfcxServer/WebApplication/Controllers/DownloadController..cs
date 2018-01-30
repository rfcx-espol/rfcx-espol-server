using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO.Packaging;

namespace WebApplication
{

    public class DownloadController : Controller {

        public string TempZipFilePath;
         

        private readonly IFileProvider _fileProvider;
        public DownloadController(IFileProvider fileProvider) {
            _fileProvider = fileProvider;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        private void CopyStream(System.IO.FileStream source, System.IO.Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }

        private void CreateZip(string tempZipPath, string inputFileToAdd)
        {
            using (Package package = Package.Open(tempZipPath, FileMode.OpenOrCreate))
            {
                string destFilename = ".\\" + Path.GetFileName(inputFileToAdd);
                Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));

                // Checking if a file already exists in the Zip 
                if (package.PartExists(uri))
                {
                    package.DeletePart(uri);
                }

                PackagePart part = package.CreatePart(uri, "", CompressionOption.Normal);

                using (FileStream fileStream = new FileStream(inputFileToAdd, FileMode.Open, FileAccess.Read))
                {
                    CopyStream(fileStream, part.GetStream());
                }
            }
        }

        public void CompressToZipArchive(List<string> filesForCompression, IFormFile file)
        {

            TempZipFilePath = Path.Combine(Core.GzipFolderPath,
                                            file.FileName);


            foreach (string fileToCompress in filesForCompression)
            {
                // Loop through each entry and add to ZipArchive  
                CreateZip(TempZipFilePath, fileToCompress);
            }
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


            var gzipFilePath = Path.Combine(Core.GzipFolderPath,
                                            file.FileName);

            

            //using (var stream = new FileStream(gzipFilePath, FileMode.Create))
            //{
            //    await file.CopyToAsync(stream);
            //}

            var gzipFileInfo = new FileInfo(gzipFilePath);
            var decompressedPath = gzipFilePath.Remove((int)(gzipFileInfo.FullName.Length - gzipFileInfo.Extension.Length));
            
            { // Decompression Test
                using (var compressedStream = new FileStream(gzipFilePath, FileMode.Open)) {
                    using (var decompressedFileStream = new FileStream(decompressedPath, FileMode.Create)) {
                        using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress)) {
                            decompressionStream.CopyTo(decompressedFileStream);
                        }
                    }
                }
            }
            
            { // Convert Decompressed File to ogg and add to playlist
                var process = new Process();
                process.StartInfo.FileName = "ffmpeg";
                var decompressedFileInfo = new FileInfo(decompressedPath);
                var filename = decompressedFileInfo.Name.Remove((int)(decompressedFileInfo.Name.Length - decompressedFileInfo.Extension.Length));
                // var milliseconds = long.Parse(filename);
                // var date = DateTimeExtensions.DateTimeFromMilliseconds(milliseconds);
                // var localDate = date.ToLocalTime();
                var oggFilename = filename + ".ogg";
                var oggFilePath = Path.Combine(Core.OggFolderPath, oggFilename) ;
                process.StartInfo.Arguments = "-i " + decompressedPath + " " + oggFilePath;
                process.Start();
            }

            return Content("File receivedaaaa");
        }
    }
}