using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using System;
using Microsoft.Extensions.Primitives;

namespace WebApplication {

    public class Thing {
        public string fileName;
        public string deviceId;
    }

    public class GZipController : Controller {

        public class DeviceFile {
            public KeyValueAccumulator formAccumulator;
            public MemoryStream memoryStream;

            public DeviceFile() {
                formAccumulator = new KeyValueAccumulator();
                memoryStream = new MemoryStream();
            }
        }

        private readonly IFileProvider _fileProvider;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        public GZipController(IFileProvider fileProvider) {
            _fileProvider = fileProvider;
        }

        public IActionResult Index() {
            return View();
        }
        
        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }

        public async Task<DeviceFile> HandleMultipartRequest() {
            var deviceFile = new DeviceFile();

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null) {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        await section.Body.CopyToAsync(deviceFile.memoryStream);
                        deviceFile.memoryStream.Seek(0, SeekOrigin.Begin);
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Content-Disposition: form-data; name="key"
                        //
                        // value

                        // Do not limit the key name length here because the 
                        // multipart headers length limit is already in effect.
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            deviceFile.formAccumulator.Append(key.Value, value);

                            if (deviceFile.formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }
            return deviceFile;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile() {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var deviceFile = await HandleMultipartRequest();
            var formData = deviceFile.formAccumulator.GetResults();
            StringValues filename;
            bool ok = false;
            ok = formData.TryGetValue("filename", out filename);
            if (!ok) {
                return BadRequest("Expected filename key");
            }
            StringValues deviceId;
            ok = formData.TryGetValue("deviceId", out deviceId);
            if (!ok) {
                return BadRequest("Expected deviceId key");
            }
            
            {
                Core.DeviceDictionary.TryAdd(deviceId.ToString(), Core.DeviceDictionary.Count);
                Core.SaveDeviceDictionaryToFile();
            }


            {
                string strDeviceId = "";
                int id;
                if (Core.DeviceDictionary.TryGetValue(deviceId.ToString(), out id)) {
                    strDeviceId = id.ToString();
                }
                string strfilename = filename.ToString();
                Core.MakeDeviceFolder(strDeviceId);
                var gzipFilePath = Path.Combine(Core.DeviceGzipFolderPath(strDeviceId),
                                                strfilename);
                
                using (var stream = new FileStream(gzipFilePath, FileMode.Create)) {
                    await deviceFile.memoryStream.CopyToAsync(stream);
                    deviceFile.memoryStream.Close();
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
                    var filenameNoExtension = decompressedFileInfo.Name.Remove((int)(decompressedFileInfo.Name.Length - decompressedFileInfo.Extension.Length));
                    // var milliseconds = long.Parse(filenameNoExtension);
                    // var date = DateTimeExtensions.DateTimeFromMilliseconds(milliseconds);
                    // var localDate = date.ToLocalTime();
                    var oggFilename = filenameNoExtension + ".ogg";
                    var oggFilePath = Path.Combine(Core.DeviceOggFolderPath(strDeviceId), oggFilename);
                    process.StartInfo.Arguments = "-i " + decompressedPath + " " + oggFilePath;
                    process.Start();
                }
            }
            return Content("File received");

        }
        /*
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
        }*/
    }
}