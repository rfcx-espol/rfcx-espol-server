using WebApplication.IRepository;
using WebApplication.DbModels;
using WebApplication.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using System;
public class ImageRepository : IImageRepository
{
    private readonly ObjectContext _context =null; 

    public ImageRepository(IOptions<Settings> settings)
    {
        _context = new ObjectContext(settings);
    } 

    
    public async Task<Image> Find(string _id)
    {
        var filter = "{'_id':" +  "ObjectId('"+_id + "')}";
        var imgDB = await _context.Image.Find(filter).Limit(1).FirstOrDefaultAsync();
        return imgDB;
    }

    private bool IsApiKeyCorrect(string ApiKey)
    {
        var filter = Builders<Station>.Filter.Eq("APIKey", ApiKey);
        return _context.Stations.Find(filter).Any();
    }
    public async Task<ActionResult> PostPicture(ImageRequest req)
    {
        if(IsApiKeyCorrect(req.APIKey)){
            string extension = System.IO.Path.GetExtension(req.ImageFile.FileName);
            Image img = new Image(req.StationId, req.CaptureDate, extension);
            var imgPath = Constants.RUTA_ARCHIVOS_ANALISIS_IMAGENES + img.StationId + "/" + img.Path;
            new FileInfo(imgPath).Directory.Create();
            using(FileStream stream = new FileStream(imgPath, FileMode.Create)){
                await req.ImageFile.CopyToAsync(stream);
            }
            _context.Image.InsertOne(img);
            return new ContentResult()
            {
                Content = "{\"_id\": \"" + img.id + "\"}",
                ContentType="application/json"
            };
        }else{
            return new StatusCodeResult(500);
        }
    }

    public async Task<List<Image>> ListImages(DateTime starttime, DateTime endtime, int page, int rows, int stationid)
    {
        var filterBuilder = Builders<Image>.Filter;
        var start = starttime;
        var end = endtime;
        var filter = filterBuilder.Gte(x => x.CaptureDate, new BsonDateTime(start)) & filterBuilder.Lte(x => x.CaptureDate, new BsonDateTime(end)) &
        filterBuilder.Eq(x => x.StationId, stationid);
        var arr = new List<Image>();
        await _context.Image.Find(filter).ForEachAsync(
            img =>
            {
                arr.Add(img);
            });
        var lower = rows * (page - 1);
        if (lower >= arr.Count)
            return new List<Image>();
        var upper = lower + rows;
        if(upper > arr.Count)
            upper = arr.Count;  
        return arr.GetRange(lower, upper-lower);
        

    }

    public void ChangeFamily(Image image, ImageRequest request)
    {
        image.Family = new string[request.Family.Count];
        for (int i  = 0; i < request.Family.Count; i++)
        {
            image.Family[i] = request.Family[i];
        }
        var filter = Builders<Image>.Filter.Eq("_id", image.id);
        var updateDef = Builders<Image>.Update.Set("family", image.Family);
        var result = _context.Image.UpdateOne(filter, updateDef);
    }
}