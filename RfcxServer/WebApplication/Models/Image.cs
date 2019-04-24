using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;




namespace WebApplication.Models
{
    [BsonIgnoreExtraElements]
    public class Image
    {
        public static IMongoClient client = new MongoClient(Constants.MONGO_CONNECTION);
        public static IMongoDatabase _database = client.GetDatabase(Constants.DEFAULT_DATABASE_NAME);
        public static IMongoCollection<Image> collection = _database.GetCollection<Image>("Camera_Image");
        
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        public ObjectId id {get; set;}
        public string ImageId { get; set; }
        public int Id {get; set;}
        public int StationId{ get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CaptureDate{ get; set; }

        public DateTime ArriveDate{ get; set; }
        
        public string Path{ get; set; }
        public string State{ get; set; }
        public string[] Family{ get; set; }
        public List<Station> Stations { get; set; }
        public List<String> LabelList { get; set; }
        

        public static async Task<Image> Find(int id){
            var filter = "{_id:" + id + "}";
            var imgDB = await collection.Find(filter).Limit(1).FirstOrDefaultAsync();
            return imgDB;
        }

        public Image(){}
        public Image(int IdEstacion, string FechaCaptura)
        {
            id = ObjectId.GenerateNewId();
            this.StationId = IdEstacion;
            this.CaptureDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(FechaCaptura)).DateTime;
            Path = Constants.RUTA_ARCHIVOS + "images/" + this.StationId + "/" + id + ".jpg";
            State = "PENDIENTE";
        }
        public static void PostPicture(string FechaCaptura, int IdEstacion, string base64Image){
            Image img = new Image(IdEstacion, FechaCaptura);
            var bytes = Convert.FromBase64String(base64Image);
            new FileInfo(img.Path).Directory.Create();
            using (var imageFile = new FileStream(img.Path, FileMode.Create))
            {
                imageFile.Write(bytes ,0, bytes.Length);
                imageFile.Flush();
            }
            collection.InsertOne(img);
        }
        
    }
    
}