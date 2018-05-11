using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace WebApplication.Models
{

    public class Sensor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SensorId { get; set; }
        public string Nombre { get; set; }
        public string Funcion { get; set; }
    }
    
}