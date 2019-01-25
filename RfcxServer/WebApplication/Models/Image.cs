using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
namespace WebApplication.Models
{
    [BsonIgnoreExtraElements]
    public class Image
    {
        public static IMongoClient client;
        public static IMongoDatabase _database;
        public static IMongoCollection<Image> collection;
        static Image()
        {
            
            client = new MongoClient(Constants.MONGO_CONNECTION);
            _database = client.GetDatabase(Constants.DEFAULT_DATABASE_NAME);
            var collectionString = "Camera_Image";
            BsonDefaults.GuidRepresentation = GuidRepresentation.CSharpLegacy;
            if(!CollectionExists(collectionString))
            {
                _database.CreateCollection(collectionString);
            }

            collection = _database.GetCollection<Image>("Camera_Image");
        }
        
        
        [BsonId]
        public ObjectId id {get; set;}
        public int StationId{ get; set; }
        public DateTime CaptureDate{ get; set; }
        public string Path{ get; set; }
        public string State{ get; set; }
        public string[] Family{ get; set; }
        
        public static async Task<Image> Find(string _id){
            var filter = "{'_id':" +  "ObjectId('"+_id + "')}";
            var imgDB = await collection.Find(filter).Limit(1).FirstOrDefaultAsync();
            return imgDB;
        }

        public Image(){}
        public Image(int IdEstacion, string FechaCaptura, string Extension)
        {
            id = ObjectId.GenerateNewId();
            this.StationId = IdEstacion;
            this.CaptureDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(FechaCaptura)).DateTime;
            Path = id + Extension;
            State = "PENDIENTE";
        }
        public static async Task<ActionResult> PostPicture(ImageRequest req){
            if(IsApiKeyCorrect(req.APIKey)){
                string extension = System.IO.Path.GetExtension(req.ImageFile.FileName);
                Image img = new Image(req.StationId, req.CaptureDate, extension);
                var imgPath = Constants.RUTA_ARCHIVOS_ANALISIS_IMAGENES + img.StationId + "/" + img.Path;
                new FileInfo(imgPath).Directory.Create();
                using(FileStream stream = new FileStream(imgPath, FileMode.Create)){
                    await req.ImageFile.CopyToAsync(stream);
                }
                collection.InsertOne(img);
                return new ContentResult()
                {
                    Content = "{\"_id\": \"" + img.id + "\"}",
                    ContentType="application/json"
                };
            }else{
                return new StatusCodeResult(500);
            }
        }

        public static bool CollectionExists(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = _database.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collections.Any();
        }
       
        private static bool IsApiKeyCorrect(string ApiKey){
            var StationCollection = _database.GetCollection<Station>("Station");
            var filter = Builders<Station>.Filter.Eq("APIKey", ApiKey);
            return StationCollection.Find(filter).Any();
        }

    }
    
}