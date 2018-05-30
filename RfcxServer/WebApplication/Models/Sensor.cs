using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace WebApplication.Models
{

    public class Sensor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SensorId { get; set; }
        public int Id { get; set; }
        public int DispositivoId { get; set; }
        public string Tipo { get; set; }
        public string Ubicacion { get; set; }
    }
    
}