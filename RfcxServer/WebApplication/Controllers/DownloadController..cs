using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;

namespace WebApplication {

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
        public async Task<IActionResult> UploadFile(IFormFile file) {
            if (file == null || file.Length == 0)
                return Content("File not selected");

            var gzipFilePath = Path.Combine(Core.GzipFolderPath,
                                            file.FileName);

            

            using (var stream = new FileStream(gzipFilePath, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

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

            return Content("File received");
        }
    }
}