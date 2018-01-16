using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;

namespace WebApplication {

    public class GZipController : Controller {

        private readonly IFileProvider _fileProvider;
        public GZipController(IFileProvider fileProvider) {
            _fileProvider = fileProvider;
        }

        public IActionResult Index() {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file) {
            if (file == null || file.Length == 0)
                return Content("File not selected");

            var path = Path.Combine(Core.FilesFolderPath,
                                    file.FileName);

            using (var stream = new FileStream(path, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

            var fileInfo = new FileInfo(path);
            var decompressedPath = path.Remove((int)(fileInfo.FullName.Length - fileInfo.Extension.Length));

            { // Decompression Test
                using (var compressedStream = new FileStream(path, FileMode.Open)) {
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
                var flacFileInfo = new FileInfo(decompressedPath);
                var oggPath = flacFileInfo.FullName.Remove((int)(flacFileInfo.FullName.Length - flacFileInfo.Extension.Length)) + ".ogg";
                process.StartInfo.Arguments = "-i " + decompressedPath + " " + oggPath;
                process.Start();
            }

            return Content("File received");
        }
    }
}