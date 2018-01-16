using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

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

                var playlist_path = Path.Combine(Directory.GetCurrentDirectory(), "icecast", "playlist", "playlist.txt");
                using (StreamWriter sw = new StreamWriter("playlist_path",false))
                {
                   sw.WriteLine(oggPath + "/n");
                }
            }

            return Content("File received");
        }
    }
}