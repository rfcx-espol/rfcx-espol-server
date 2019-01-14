using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.IO;
namespace WebApplication.Models
{
    [BsonIgnoreExtraElements]
    public class Image
    {
        public static IMongoClient client = new MongoClient(Constants.MONGO_CONNECTION);
        public static IMongoDatabase _database = client.GetDatabase(Constants.DEFAULT_DATABASE_NAME);
        public static IMongoCollection<Image> collection = _database.GetCollection<Image>("Camera_Image");
        
        [BsonId]
        public ObjectId id {get; set;}
        public int IdEstacion{ get; set; }
        public DateTime FechaCaptura{ get; set; }
        public string Ruta{ get; set; }
        public string Estado{ get; set; }
        public string[] Familia{ get; set; }
        
        public static async Task<Image> Find(int id){
            var filter = "{_id:" + id + "}";
            var imgDB = await collection.Find(filter).Limit(1).FirstOrDefaultAsync();
            return imgDB;
        }

        public Image(){}
        public Image(int IdEstacion, string FechaCaptura, string Extension)
        {
            id = ObjectId.GenerateNewId();
            this.IdEstacion = IdEstacion;
            this.FechaCaptura = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(FechaCaptura)).DateTime;
            Ruta = Constants.RUTA_ARCHIVOS + "images/" + this.IdEstacion + "/" + id + Extension;
            Estado = "PENDIENTE";
        }
        public static async Task PostPicture(ImageRequest req){
            string extension = Path.GetExtension(req.ImageFile.FileName);
            Image img = new Image(req.IdEstacion, req.FechaCaptura, extension);
            using(FileStream stream = new FileStream(img.Ruta, FileMode.Create)){
                await req.ImageFile.CopyToAsync(stream);
            }
            collection.InsertOne(img);
        }

       
        

    }
    
}