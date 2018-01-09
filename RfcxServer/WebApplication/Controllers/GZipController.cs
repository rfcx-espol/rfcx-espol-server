using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;

namespace WebApplication {

    public class GZipController : Controller {
        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file) {
            if (file == null || file.Length == 0)
                return Content("File not selected");
 
            var path = Path.Combine(Directory.GetCurrentDirectory(), 
                                    "uploaded",
                                    file.FileName);
 
            using (var stream = new FileStream(path, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

            { // Decompression Test
                var fileInfo = new FileInfo(path);
                using (var compressedStream = new FileStream(path, FileMode.Open)) {
                    var decompressedPath = path.Remove((int)(fileInfo.FullName.Length - fileInfo.Extension.Length));
                    using (var decompressedFileStream = new FileStream(decompressedPath, FileMode.Create)) {
                        using (var decompressionStream = new GZipStream(compressedStream, CompressionMode.Decompress)) {
                            decompressionStream.CopyTo(decompressedFileStream);
                        }
                    }
                }
            }
            
            { // Convert Decompressed File to ogg and add to playlist
                
            }

            return Content("File received");
        }
    }
}