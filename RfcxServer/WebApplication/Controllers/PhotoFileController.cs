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
        private readonly ISpecieRepository _SpecieRepository;

        public class SpecieFile {
            public KeyValueAccumulator formAccumulator;
            public MemoryStream memoryStream;
            public SpecieFile() {
                formAccumulator = new KeyValueAccumulator();
                memoryStream = new MemoryStream();
            }
        }

        private readonly IFileProvider _fileProvider;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        public PhotoFileController(IFileProvider fileProvider, ISpecieRepository SpecieRepository) {
            _fileProvider = fileProvider;
             _SpecieRepository=SpecieRepository;
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

        public async Task<SpecieFile> HandleMultipartRequest() {
            var specieFile = new SpecieFile();

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
                        await section.Body.CopyToAsync(specieFile.memoryStream);
                        specieFile.memoryStream.Seek(0, SeekOrigin.Begin);
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
                            specieFile.formAccumulator.Append(key.Value, value);

                            if (specieFile.formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
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
            return specieFile;
        }

        [HttpPost("CreateSpecie")]
        public async Task<IActionResult> CreateSpecie() {
            var SpecieData = await HandleMultipartRequest();
            var formData = SpecieData.formAccumulator.GetResults();
            bool ok = false;
            StringValues nombre_especie;
            StringValues familia;
            ok = formData.TryGetValue("nombre_especie", out nombre_especie);
            if (!ok) {
                return BadRequest("Expected specie name");
            }
            ok = formData.TryGetValue("familia", out familia);
            if (!ok) {
                return BadRequest("Expected family name");
            }
            Specie spe = new Specie();
            spe.Name = nombre_especie;
            spe.Family = familia;
            Task result;
            result = _SpecieRepository.Add(spe);
            return Content("Specie Created successfully");
        }

        [HttpPost("UploadPhoto")]
        public async Task<IActionResult> UploadPhoto() {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var specieFile = await HandleMultipartRequest();
            var formData = specieFile.formAccumulator.GetResults();
            bool ok = false;
            StringValues Filename;
            StringValues SpecieId;

            ok = formData.TryGetValue("SpecieId", out SpecieId);
            if (!ok) {
                return BadRequest("Expected ID key");
            }

            {
                var SpecieResult = _SpecieRepository.Get(Int32.Parse(SpecieId));
                int specie_id = SpecieResult.Result.Id;
                string str_specie_id = specie_id.ToString();
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

                if(str_specie_id != null){
                    Core.MakeSpecieFolder(str_specie_id);
                    filePath = Path.Combine(Core.SpecieFolderPath(str_specie_id), strfilename);
                    Console.Write(filePath);
                }

                var photo =new Photo();
                photo.Description = description;
                photo.Filename = strfilename;
                Task result;
                result = _PhotoRepository.Add(photo);
                result = _SpecieRepository.AddPhoto(Int32.Parse(str_specie_id), photo);

                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await specieFile.memoryStream.CopyToAsync(stream);
                    specieFile.memoryStream.Close();
                }
                     
            }
            return Content("Photo received");
        }
        
    }
}