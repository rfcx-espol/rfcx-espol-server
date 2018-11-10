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
using WebApplication.Models;
using WebApplication.Controllers;
using WebApplication.Repository;
using WebApplication.IRepository;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace WebApplication {

    [Route("[controller]")]
    public class PhotoFileController : Controller {
        private readonly IPhotoRepository _PhotoRepository;
        private readonly IKindRepository _KindRepository;

        public class KindFile {
            public KeyValueAccumulator formAccumulator;
            public MemoryStream memoryStream;
            public KindFile() {
                formAccumulator = new KeyValueAccumulator();
                memoryStream = new MemoryStream();
            }
        }

        private readonly IFileProvider _fileProvider;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        public PhotoFileController(IFileProvider fileProvider, IPhotoRepository PhotoRepository, IKindRepository KindRepository) {
            _fileProvider = fileProvider;
             _PhotoRepository=PhotoRepository;
             _KindRepository=KindRepository;
        }

        [HttpGet]
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

        public async Task<KindFile> HandleMultipartRequest() {
            var kindFile = new KindFile();

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
                        await section.Body.CopyToAsync(kindFile.memoryStream);
                        kindFile.memoryStream.Seek(0, SeekOrigin.Begin);
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
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
                            kindFile.formAccumulator.Append(key.Value, value);

                            if (kindFile.formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
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
            return kindFile;
        }

        [HttpPost("UploadPhoto")]
        public async Task<IActionResult> UploadPhoto() {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var kindFile = await HandleMultipartRequest();
            var formData = kindFile.formAccumulator.GetResults();
            bool ok = false;
            StringValues Filename;
            StringValues KindId;

            ok = formData.TryGetValue("KindId", out KindId);
            if (!ok) {
                return BadRequest("Expected ID key");
            }

            {
                var KindResult = _KindRepository.Get(Int32.Parse(KindId));
                int kind_id = KindResult.Result.Id;
                string str_kind_id = kind_id.ToString();
                var filePath = "";
                
                ok = formData.TryGetValue("Filename", out Filename);
                if (!ok) {
                    return BadRequest("Expected filename key");
                }

                StringValues description;
                ok = formData.TryGetValue("Description", out description);
                if (!ok) {
                    return BadRequest("Expected Description key");
                }
                
                string strfilename = Filename.ToString();

                if(str_kind_id != null){
                    Core.MakeKindFolder(str_kind_id);
                    filePath = Path.Combine(Core.KindFolderPath(str_kind_id), strfilename);
                    Console.Write(filePath);
                }

                var photo =new Photo();
                photo.KindId = kind_id;
                photo.Description = description;
                Task result;
                result = _PhotoRepository.Add(photo);

                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await kindFile.memoryStream.CopyToAsync(stream);
                    kindFile.memoryStream.Close();
                }
                     
            }
            return Content("Photo received");
        }
        
    }
}